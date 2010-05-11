// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Pull parser which uses Grammar table to parse input stream.
	/// </summary>
	public sealed class LalrProcessor: IParser {
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

		private readonly ITokenizer tokenizer;
		private readonly Stack<Token> tokenStack; // Stack of LR states used for LR parsing.
		private readonly bool trim;
		private LalrState currentState;
		private TextToken currentToken;

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		/// <param name="initialLalrState">Initial state of the lalr.</param>
		public LalrProcessor(ITokenizer tokenizer, LalrState initialLalrState) : this(tokenizer, initialLalrState, false) {}

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		/// <param name="initialLalrState">Initial state of the lalr.</param>
		/// <param name="trim">if set to <c>true</c> [trim].</param>
		public LalrProcessor(ITokenizer tokenizer, LalrState initialLalrState, bool trim)
			: base() {
			if (tokenizer == null) {
				throw new ArgumentNullException("tokenizer");
			}
			if (initialLalrState == null) {
				throw new ArgumentNullException("initialLalrState");
			}
			this.tokenizer = tokenizer;
			currentState = initialLalrState;
			this.trim = trim;
			tokenStack = new Stack<Token>();
			tokenStack.Push(new RootToken(initialLalrState));
		}

		/// <summary>
		/// Gets the current currentToken.
		/// </summary>
		/// <value>The current currentToken.</value>
		public Token CurrentToken {
			get {
				if (currentToken != null) {
					return currentToken;
				}
				if (tokenStack.Count > 0) {
					return tokenStack.Peek();
				}
				return null;
			}
		}

		public bool Trim {
			get {
				return trim;
			}
		}

		bool IParser.CanTrim(Rule rule) {
			return trim;
		}

		Token IParser.PopToken() {
			return tokenStack.Pop();
		}

		void IParser.PushToken(Token token) {
			Debug.Assert(token != null);
			tokenStack.Push(token);
		}

		Token IParser.CreateReduction(Rule rule) {
			Debug.Assert(rule != null);
			Token[] tokens = new Token[rule.SymbolCount];
			for (int i = tokens.Length-1; i >= 0; i--) {
				tokens[i] = tokenStack.Pop();
			}
			return new Reduction(rule, tokens);
		}

		void IParser.SetState(LalrState state) {
			Debug.Assert(state != null);
			currentState = state;
		}

		Token IParser.TopToken {
			get {
				return tokenStack.Peek();
			}
		}

		/// <summary>
		/// Gets array of expected currentToken symbols.
		/// </summary>
		public ReadOnlyCollection<Symbol> GetExpectedTokens() {
			List<Symbol> expectedTokens = new List<Symbol>(currentState.ActionCount);
			for (int i = 0; i < currentState.ActionCount; i++) {
				switch (currentState.GetAction(i).Symbol.Kind) {
				case SymbolKind.Terminal:
				case SymbolKind.End:
					expectedTokens.Add(currentState.GetAction(i).Symbol);
					break;
				}
			}
			return expectedTokens.AsReadOnly();
		}

		/// <summary>
		/// Executes next step of parser and returns parser currentState.
		/// </summary>
		/// <returns>Parser current currentState.</returns>
		public ParseMessage Parse() {
			while (true) {
				TextToken inputToken;
				if (currentToken == null) {
					//We must read a currentToken
					ParseMessage message = tokenizer.NextToken(out inputToken);
					//					Debug.WriteLine(string.Format("State: {0} Line: {1}, Column: {2}, Parse Value: {3}, Token Type: {4}", currentState.Index, inputToken.Line, inputToken.LinePosition, inputToken.Text, inputToken.ParentSymbol.Name), "Token Read");
					if (inputToken.ParentSymbol.Kind != SymbolKind.End) {
						currentToken = inputToken;
						return message;
					}
				} else {
					inputToken = currentToken;
				}
				switch (inputToken.ParentSymbol.Kind) {
				case SymbolKind.WhiteSpace:
				case SymbolKind.CommentStart:
				case SymbolKind.CommentLine:
					currentToken = null;
					break;
				case SymbolKind.Error:
					return ParseMessage.LexicalError;
				default:
					LalrAction action = currentState.GetActionBySymbol(inputToken.ParentSymbol);
					if (action == null) {
						return ParseMessage.SyntaxError;
					}
					switch (action.Execute(this, inputToken)) {
						// ParseToken() equivalent
					case TokenParseResult.Accept:
						return ParseMessage.Accept;
					case TokenParseResult.Shift:
						//						Debug.WriteLine(string.Format("State: {0}", currentState.Index), "Shift");
						currentToken = null;
						break;
					case TokenParseResult.SyntaxError:
						return ParseMessage.SyntaxError;
					case TokenParseResult.ReduceNormal:
						//						Debug.WriteLine(string.Format("State: {0}, Token Type: {1}", currentState.Index, action.Target), "Reduce");
						return ParseMessage.Reduction;
					case TokenParseResult.InternalError:
						return ParseMessage.InternalError;
					}
					break;
				}
			}
		}
	}
}