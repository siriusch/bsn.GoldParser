// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public class Tokenizer: ITokenizer {
		private readonly CharBuffer buffer; // Buffer to keep current characters.
		private readonly Symbol endSymbol;
		private readonly Symbol errorSymbol;
		private readonly DfaState initialState;
		private int lineNumber;
		private int linePosition;

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="textReader"><see cref="TextReader"/> instance to read data from.</param>
		/// <param name="initialState">The initial DFA state</param>
		/// <param name="endSymbol">The symbol to be used when the end is reached (EOF)</param>
		/// <param name="errorSymbol">The symbol to be used for lexical errors</param>
		public Tokenizer(TextReader textReader, DfaState initialState, Symbol endSymbol, Symbol errorSymbol) {
			if (textReader == null) {
				throw new ArgumentNullException("textReader");
			}
			if (initialState == null) {
				throw new ArgumentNullException("initialState");
			}
			if (endSymbol == null) {
				throw new ArgumentNullException("endSymbol");
			}
			buffer = new CharBuffer(textReader);
			linePosition = 1;
			lineNumber = 1;
			this.endSymbol = endSymbol;
			this.errorSymbol = errorSymbol ?? endSymbol;
			this.initialState = initialState;
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
		/// Gets the index of the input.
		/// </summary>
		/// <value>The index of the input.</value>
		public int InputIndex {
			get {
				return buffer.Position;
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

		/// <summary>
		/// Reads next token from the input stream.
		/// </summary>
		/// <returns>Token symbol which was read.</returns>
		public ParseMessage NextToken(out TextToken token) {
			using (CharBuffer.Mark mark = buffer.CreateMark()) {
				using (CharBuffer.Mark acceptMark = buffer.CreateMark()) {
					ParseMessage result = ParseMessage.None;
					LineInfo tokenPosition = new LineInfo(InputIndex, lineNumber, linePosition);
					List<int> lineBreakPositions = null;
					Symbol tokenSymbol = null;
					DfaState state = initialState;
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
								tokenSymbol = errorSymbol;
							} else {
								buffer.StepBack(1);
							}
							break;
						}
						// This code checks whether the target state accepts a token. If so, it sets the
						// appropiate variables so when the algorithm in done, it can return the proper
						// token and number of characters.
						if (state.AcceptSymbol != null) {
							acceptMark.MoveToReadPosition();
							tokenSymbol = state.AcceptSymbol;
						}
					}
					if (tokenSymbol == null) {
						acceptMark.MoveToReadPosition();
						tokenSymbol = endSymbol;
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
						SymbolKind kind;
						do {
							kind = NextToken(out token) != ParseMessage.None ? token.ParentSymbol.Kind : SymbolKind.Error;
						} while ((kind != SymbolKind.End) && (kind != SymbolKind.Error) && (kind != SymbolKind.CommentEnd));
						result = (kind == SymbolKind.CommentEnd) ? ParseMessage.CommentBlockRead : ParseMessage.CommentError;
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
					token = new TextToken(tokenSymbol, mark.Text, tokenPosition);
					return result;
				}
			}
		}
	}
}