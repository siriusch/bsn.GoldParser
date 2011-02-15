// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-goldparser.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.IO;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	///<summary>
	/// The tokenizer reads an input character stream and outputs the tokens read.
	///</summary>
	/// <remarks>
	/// A pull-model is used for the tokenizer.
	/// </remarks>
	public abstract class Tokenizer<T>: ITokenizer<T> where T: class, IToken {
		private enum ParseMode {
			SingleSymbol,
			MergeLexicalErrors,
			BlockComment
		}

		private readonly TextBuffer buffer; // Buffer to keep current characters.
		private readonly CompiledGrammar grammar;
		private bool mergeLexicalErrors;

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="textReader"><see cref="TextReader"/> instance to read data from.</param>
		/// <param name="grammar">The grammar used for the DFA states</param>
		protected Tokenizer(TextReader textReader, CompiledGrammar grammar) {
			this.grammar = grammar;
			if (textReader == null) {
				throw new ArgumentNullException("textReader");
			}
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			buffer = new TextBuffer(textReader);
		}

		/// <summary>
		/// Gets the index of the input.
		/// </summary>
		/// <value>The index of the input.</value>
		public long InputIndex {
			get {
				return buffer.Position;
			}
		}

		/// <summary>
		/// Gets current char position in the current source line. It is 1-based.
		/// </summary>
		public int LineColumn {
			get {
				return buffer.Column;
			}
		}
		/// <summary>
		/// Gets current line number. It is 1-based.
		/// </summary>
		public int LineNumber {
			get {
				return buffer.Line;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether lexical errors are merged (so that they contain more than one character).
		/// </summary>
		/// <value><c>true</c> if lexical errors are to be merged; otherwise, <c>false</c>.</value>
		public bool MergeLexicalErrors {
			get {
				return mergeLexicalErrors;
			}
			set {
				mergeLexicalErrors = value;
			}
		}

		/// <summary>
		/// Gets source of parsed data.
		/// </summary>
		public TextReader TextReader {
			get {
				return buffer.TextReader;
			}
		}

		protected abstract T CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text);

		private ParseMessage NextSymbol(ParseMode mode, out Symbol tokenSymbol, ref int length) {
			DfaState state = grammar.DfaInitialState;
			char ch;
			tokenSymbol = null;
			int offset = length;
			while (buffer.TryLookahead(ref offset, out ch)) {
				state = state.GetTransition(ch);
				// This block-if statement checks whether an edge was found from the current state.
				// If so, the state and current position advance. Otherwise it is time to exit the main loop
				// and report the token found (if there was it fact one). If the LastAcceptState is -1,
				// then we never found a match and the Error Token is created. Otherwise, a new token
				// is created using the Symbol in the Accept State and all the characters that
				// comprise it.
				if (state == null) {
					// end has been reached
					if (tokenSymbol == null) {
						//Tokenizer cannot recognize symbol
						offset = ++length;
						if (mode == ParseMode.MergeLexicalErrors) {
							while (NextSymbol(ParseMode.SingleSymbol, out tokenSymbol, ref offset) == ParseMessage.LexicalError) {
								length = offset;
							}
						}
						tokenSymbol = grammar.ErrorSymbol;
					}
					break;
				}
				if ((mode == ParseMode.BlockComment) && (!grammar.BlockCommentStates.Contains(state))) {
					state = grammar.DfaInitialState;
				} else {
					// This code checks whether the target state accepts a token. If so, it sets the
					// appropiate variables so when the algorithm in done, it can return the proper
					// token and number of characters.
					if (state.AcceptSymbol != null) {
						tokenSymbol = state.AcceptSymbol;
						length = offset;
					}
				}
			}
			if (tokenSymbol == null) {
				if (offset == length) {
					tokenSymbol = grammar.EndSymbol;
				} else {
					tokenSymbol = grammar.ErrorSymbol;
					length++;
				}
			}
			switch (tokenSymbol.Kind) {
			case SymbolKind.CommentLine:
				while (buffer.TryLookahead(length, out ch) && (ch != '\r') && (ch != '\n')) {
					length++;
				}
				return ParseMessage.CommentLineRead;
			case SymbolKind.CommentStart:
				for (;;) {
					Symbol blockTokenSymbol;
					NextSymbol(ParseMode.BlockComment, out blockTokenSymbol, ref length);
					switch (blockTokenSymbol.Kind) {
					case SymbolKind.End:
						return ParseMessage.CommentError;
					case SymbolKind.CommentEnd:
						return ParseMessage.CommentBlockRead;
					}
				}
			case SymbolKind.Error:
				return ParseMessage.LexicalError;
			}
			return ParseMessage.TokenRead;
		}

		/// <summary>
		/// Gets the grammar.
		/// </summary>
		/// <value>The grammar.</value>
		public CompiledGrammar Grammar {
			get {
				return grammar;
			}
		}

		/// <summary>
		/// Reads next token from the input stream.
		/// </summary>
		/// <returns>Token symbol which was read.</returns>
		public virtual ParseMessage NextToken(out T token) {
			Symbol tokenSymbol;
			int offset = 0;
			ParseMessage result = NextSymbol(MergeLexicalErrors ? ParseMode.MergeLexicalErrors : ParseMode.SingleSymbol, out tokenSymbol, ref offset);
			LineInfo position;
			string text = buffer.Read(offset, out position);
			token = CreateToken(tokenSymbol, position, text);
			return result;
		}
	}

	/// <summary>
	/// A concrete tokenizer creating the normal <see cref="TextToken"/> as tokens.
	/// </summary>
	public class Tokenizer: Tokenizer<Token> {
		/// <summary>
		/// Initializes a new instance of the <see cref="Tokenizer"/> class.
		/// </summary>
		/// <param name="textReader"><see cref="TextReader"/> instance to read data from.</param>
		/// <param name="grammar">The grammar used for the DFA states</param>
		public Tokenizer(TextReader textReader, CompiledGrammar grammar): base(textReader, grammar) {}

		protected override Token CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
			return new TextToken(tokenSymbol, tokenPosition, text);
		}
	}
}
