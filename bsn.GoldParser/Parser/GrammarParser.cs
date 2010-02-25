// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Pull parser which uses Grammar table to parse input stream.
	/// </summary>
	public sealed class GrammarParser: IParser {
		private class RootToken: Token {
			public RootToken(LalrState state) {
				State = state;
			}

			public override Symbol ParentSymbol {
				get {
					return null;
				}
			}

			public override LineInfo Position {
				get {
					return default(LineInfo);
				}
			}
		}

		private readonly CharBuffer buffer; // Buffer to keep current characters.
		private readonly CompiledGrammar grammar; // Grammar of parsed language.
		private readonly Stack<Token> lalrTokenStack; // Stack of LR states used for LR parsing.
		private LalrState currentLalrState;
		private TextToken currentToken;
		private int lineNumber;
		private int linePosition;

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="textReader"><see cref="TextReader"/> instance to read data from.</param>
		/// <param name="grammar">Grammar with parsing tables to parser input stream.</param>
		public GrammarParser(TextReader textReader, CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			buffer = new CharBuffer(textReader);
			linePosition = 1;
			lineNumber = 1;
			currentLalrState = grammar.InitialLRState;
			lalrTokenStack = new Stack<Token>();
			lalrTokenStack.Push(new RootToken(currentLalrState));

			this.grammar = grammar;
		}

		/// <summary>
		/// Gets the current token.
		/// </summary>
		/// <value>The current token.</value>
		public Token CurrentToken {
			get {
				if (currentToken != null) {
					return currentToken;
				}
				if (lalrTokenStack.Count > 0) {
					return lalrTokenStack.Peek();
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the parser's grammar.
		/// </summary>
		public CompiledGrammar Grammar {
			get {
				return grammar;
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
		/// Gets source of parsed data.
		/// </summary>
		public TextReader TextReader {
			get {
				return buffer.TextReader;
			}
		}

		bool IParser.CanTrim(Rule rule) {
			return false;
		}

		Token IParser.PopToken() {
			return lalrTokenStack.Pop();
		}

		void IParser.PushToken(Token token) {
			Debug.Assert(token != null);
			lalrTokenStack.Push(token);
		}

		Token IParser.CreateReduction(Rule rule) {
			Debug.Assert(rule != null);
			Token[] tokens = new Token[rule.SymbolCount];
			for (int i = tokens.Length-1; i >= 0; i--) {
				tokens[i] = lalrTokenStack.Pop();
			}
			return new Reduction(rule, tokens);
		}

		void IParser.SetState(LalrState state) {
			Debug.Assert(state != null);
			currentLalrState = state;
		}

		Token IParser.TopToken {
			get {
				return lalrTokenStack.Peek();
			}
		}

		/// <summary>
		/// Gets array of expected token symbols.
		/// </summary>
		public Symbol[] GetExpectedTokens() {
#warning implement on-demand getting
			/*
			//=== Syntax Error! Fill Expected Tokens
			expectedTokens = new Symbol[lrState.ActionCount];
			int length = 0;
			for (int i = 0; i < lrState.ActionCount; i++) {
				switch (lrState.GetAction(i).Symbol.SymbolType) {
				case SymbolType.Terminal:
				case SymbolType.End:
					expectedTokens[length++] = lrState.GetAction(i).Symbol;
					break;
				}
			}
			if (length < expectedTokens.Length) {
				Symbol[] newArray = new Symbol[length];
				Array.Copy(expectedTokens, newArray, length);
				expectedTokens = newArray;
			} */
			return new Symbol[0];
		}

		/// <summary>
		/// Executes next step of parser and returns parser state.
		/// </summary>
		/// <returns>Parser current state.</returns>
		public ParseMessage Parse() {
			while (true) {
				TextToken inputToken;
				if (currentToken == null) {
					//We must read a token
					inputToken = RetrieveToken();
					//					Debug.WriteLine(string.Format("State: {0} Line: {1}, Column: {2}, Parse Value: {3}, Token Type: {4}", currentLalrState.Index, inputToken.LineNumber, inputToken.LinePosition, inputToken.Text, inputToken.ParentSymbol.Name), "Token Read");
					switch (inputToken.ParentSymbol.Kind) {
					case SymbolType.Error:
						return ParseMessage.LexicalError;
					case SymbolType.End:
						break;
					default:
						currentToken = inputToken;
						return ParseMessage.TokenRead;
					}
				} else {
					inputToken = currentToken;
				}
				switch (inputToken.ParentSymbol.Kind) {
				case SymbolType.WhiteSpace:
				case SymbolType.CommentStart:
				case SymbolType.CommentLine:
					currentToken = null;
					break;
				case SymbolType.Error:
					return ParseMessage.LexicalError;
				default:
					LalrAction action = currentLalrState.GetActionBySymbol(inputToken.ParentSymbol);
					if (action == null) {
						return ParseMessage.SyntaxError;
					}
					switch (action.Execute(this, inputToken)) {
						// ParseToken() equivalent
					case TokenParseResult.Accept:
						return ParseMessage.Accept;
					case TokenParseResult.Shift:
						//						Debug.WriteLine(string.Format("State: {0}", currentLalrState.Index), "Shift");
						currentToken = null;
						break;
					case TokenParseResult.SyntaxError:
						return ParseMessage.SyntaxError;
					case TokenParseResult.ReduceNormal:
						//						Debug.WriteLine(string.Format("State: {0}, Token Type: {1}", currentLalrState.Index, action.Target), "Reduce");
						return ParseMessage.Reduction;
					case TokenParseResult.InternalError:
						return ParseMessage.InternalError;
					}
					break;
				}
			}
		}

		/// <summary>
		/// Reads next token from the input stream.
		/// </summary>
		/// <returns>Token symbol which was read.</returns>
		private TextToken RetrieveToken() {
			using (CharBuffer.Mark mark = buffer.CreateMark()) {
				using (CharBuffer.Mark acceptMark = buffer.CreateMark()) {
					List<int> lineBreakPositions = null;
					Symbol tokenSymbol = null;
					DfaState dfaState = grammar.DfaInitialState;
					char ch;
					while (buffer.TryReadChar(out ch)) {
						if (ch == '\n') {
							if (lineBreakPositions == null) {
								lineBreakPositions = new List<int>(10);
							}
							lineBreakPositions.Add(buffer.Position);
						}
						dfaState = dfaState.GetTransition(ch);
						// This block-if statement checks whether an edge was found from the current state.
						// If so, the state and current position advance. Otherwise it is time to exit the main loop
						// and report the token found (if there was it fact one). If the LastAcceptState is -1,
						// then we never found a match and the Error Token is created. Otherwise, a new token
						// is created using the Symbol in the Accept State and all the characters that
						// comprise it.
						if (dfaState == null) {
							// end has been reached
							if (tokenSymbol == null) {
								//Tokenizer cannot recognize symbol
								acceptMark.MoveToReadPosition();
								buffer.MoveToMark(mark);
								buffer.TryReadChar(out ch);
								tokenSymbol = grammar.ErrorSymbol;
							} else {
								buffer.StepBack(1);
							}
							break;
						}
						// This code checks whether the target state accepts a token. If so, it sets the
						// appropiate variables so when the algorithm in done, it can return the proper
						// token and number of characters.
						if (dfaState.AcceptSymbol != null) {
							acceptMark.MoveToReadPosition();
							tokenSymbol = dfaState.AcceptSymbol;
						}
					}
					if (tokenSymbol == null) {
						acceptMark.MoveToReadPosition();
						tokenSymbol = grammar.EndSymbol;
					}
					switch (tokenSymbol.Kind) {
					case SymbolType.CommentLine:
						while (buffer.TryReadChar(out ch) && (ch != '\r') && (ch != '\n')) {}
						if ((ch == '\r') || (ch == '\n')) {
							buffer.StepBack(1);
						}
						break;
					case SymbolType.CommentStart:
						SymbolType kind;
						do {
							kind = RetrieveToken().ParentSymbol.Kind;
						} while ((kind != SymbolType.End) && (kind != SymbolType.Error) && (kind != SymbolType.CommentEnd));
						break;
					default:
						buffer.MoveToMark(acceptMark);
						break;
					}
					LineInfo tokenPosition = new LineInfo(lineNumber, linePosition);
					bool updateLine = true;
					if (lineBreakPositions != null) {
						foreach (int lineBreakPosition in lineBreakPositions) {
							if (lineBreakPosition <= buffer.Position) {
								lineNumber++;
								linePosition = buffer.Position-lineBreakPosition+1;
								updateLine = false;
							}
						}
					}
					if (updateLine) {
						linePosition += mark.Distance;
					}
					return new TextToken(tokenSymbol, mark.Text, tokenPosition);
				}
			}
		}
	}
}