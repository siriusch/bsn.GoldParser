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
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	///<summary>
	/// The tokenizer reads an input character stream and outputs the tokens read.
	///</summary>
	/// <remarks>
	/// A pull-model is used for the tokenizer.
	/// </remarks>
	public abstract class Tokenizer<T>: ITokenizer<T> where T: IToken {
		private readonly CharBuffer buffer; // Buffer to keep current characters.
		private readonly CompiledGrammar grammar;
		private int lineNumber;
		private int linePosition;
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
			buffer = new CharBuffer(textReader);
			linePosition = 1;
			lineNumber = 1;
		}

		/// <summary>
		/// Gets the index of the input.
		/// </summary>
		/// <value>The index of the input.</value>
		public int InputIndex {
			get {
				return buffer.Position;
			}
		}

		/// <summary>
		/// Gets current line number. It is 1-based.
		/// </summary>
		public int LineNumber {
			get {
				return lineNumber;
			}
		}

		/// <summary>
		/// Gets current char position in the current source line. It is 1-based.
		/// </summary>
		public int LinePosition {
			get {
				return linePosition;
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

		private ParseMessage NextToken(out T token, bool blockComment, bool mergeErrors) {
			using (CharBuffer.Mark mark = buffer.CreateMark()) {
				using (CharBuffer.Mark acceptMark = buffer.CreateMark()) {
					ParseMessage result;
					LineInfo tokenPosition = new LineInfo(InputIndex, lineNumber, linePosition);
					List<int> lineBreakPositions = null;
					Symbol tokenSymbol = null;
					DfaState state = grammar.DfaInitialState;
					char ch;
					while (buffer.TryReadChar(out ch)) {
						if (ch == '\n') {
							if (lineBreakPositions == null) {
								lineBreakPositions = new List<int>(10);
							}
							lineBreakPositions.Add(buffer.Position);
						}
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
								acceptMark.MoveToReadPosition();
								buffer.MoveToMark(mark);
								buffer.TryReadChar(out ch);
								if (mergeErrors) {
									using (CharBuffer.Mark errorMark = buffer.CreateMark()) {
										while (NextToken(out token, false, false) == ParseMessage.LexicalError) {
											errorMark.MoveToReadPosition();
										}
										buffer.MoveToMark(errorMark);
									}
								}
								tokenSymbol = grammar.ErrorSymbol;
							} else {
								buffer.StepBack(1);
							}
							break;
						}
						if (blockComment && (!grammar.BlockCommentStates.Contains(state))) {
							state = grammar.DfaInitialState;
						} else {
							// This code checks whether the target state accepts a token. If so, it sets the
							// appropiate variables so when the algorithm in done, it can return the proper
							// token and number of characters.
							if (state.AcceptSymbol != null) {
								acceptMark.MoveToReadPosition();
								tokenSymbol = state.AcceptSymbol;
							}
						}
					}
					if (tokenSymbol == null) {
						tokenSymbol = (acceptMark.Distance == 0) ? grammar.EndSymbol : grammar.ErrorSymbol;
					}
					switch (tokenSymbol.Kind) {
					case SymbolKind.CommentLine:
						while (buffer.TryReadChar(out ch) && (ch != '\r') && (ch != '\n')) {}
						if ((ch == '\r') || (ch == '\n')) {
							buffer.StepBack(1);
						}
						result = ParseMessage.CommentLineRead;
						break;
					case SymbolKind.CommentStart:
						result = ParseMessage.None;
						do {
							NextToken(out token, true, false);
							switch (token.Symbol.Kind) {
							case SymbolKind.End:
								result = ParseMessage.CommentError;
								break;
							case SymbolKind.CommentEnd:
								result = ParseMessage.CommentBlockRead;
								break;
							}
						} while (result == ParseMessage.None);
						break;
					case SymbolKind.Error:
						result = ParseMessage.LexicalError;
						break;
					default:
						buffer.MoveToMark(acceptMark);
						result = ParseMessage.TokenRead;
						break;
					}
					bool updateLine = true;
					if (lineBreakPositions != null) {
						foreach (int lineBreakPosition in lineBreakPositions) {
							if (lineBreakPosition <= buffer.Position) {
								lineNumber++;
								linePosition = (buffer.Position-lineBreakPosition)+1;
								updateLine = false;
							}
						}
					}
					if (updateLine) {
						// no linebreak was encountered, so we need to move the column
						linePosition += mark.Distance;
					}
					token = CreateToken(tokenSymbol, tokenPosition, mark.Text);
					return result;
				}
			}
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
			return NextToken(out token, false, mergeLexicalErrors);
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