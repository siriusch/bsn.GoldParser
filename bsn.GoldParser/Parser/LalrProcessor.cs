// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Pull parser which uses Grammar table to parse input stream.
	/// </summary>
	public abstract class LalrProcessor<T>: IParser<T> where T: IToken {
		private readonly Stack<KeyValuePair<T, LalrState>> tokenStack; // Stack of LR states used for LR parsing.
		private readonly ITokenizer<T> tokenizer;
		private LalrState currentState;
		private T currentToken;
		private bool currentTokenValid;

		/// <summary>
		/// Initializes new instance of Parser class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		/// <param name="trim">if set to <c>true</c> [trim].</param>
		protected LalrProcessor(ITokenizer<T> tokenizer) {
			if (tokenizer == null) {
				throw new ArgumentNullException("tokenizer");
			}
			this.tokenizer = tokenizer;
			currentState = tokenizer.Grammar.InitialLRState;
			currentTokenValid = false;
			tokenStack = new Stack<KeyValuePair<T, LalrState>>();
			tokenStack.Push(new KeyValuePair<T, LalrState>(default(T), currentState));
		}

		/// <summary>
		/// Gets the current currentToken.
		/// </summary>
		/// <value>The current currentToken.</value>
		public IToken CurrentToken {
			get {
				if (currentToken != null) {
					return currentToken;
				}
				if (tokenStack.Count > 0) {
					return tokenStack.Peek().Key;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets array of expected currentToken symbols.
		/// </summary>
		public ReadOnlyCollection<Symbol> GetExpectedTokens() {
			List<Symbol> expectedTokens = new List<Symbol>(currentState.ActionCount);
#warning Do we need to recurse somehow on non-terminals?
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
				T inputToken;
				if (currentToken == null) {
					//We must read a currentToken
					T textInputToken;
					ParseMessage message = tokenizer.NextToken(out textInputToken);
					//					Debug.WriteLine(string.Format("State: {0} Line: {1}, Column: {2}, Parse Value: {3}, Token Type: {4}", currentState.Index, inputToken.Line, inputToken.LinePosition, inputToken.Text, inputToken.symbol.Name), "Token Read");
					if (textInputToken.Symbol.Kind != SymbolKind.End) {
						currentToken = textInputToken;
						return message;
					}
					inputToken = textInputToken;
				} else {
					inputToken = currentToken;
				}
				switch (inputToken.Symbol.Kind) {
				case SymbolKind.WhiteSpace:
				case SymbolKind.CommentStart:
				case SymbolKind.CommentLine:
					ClearCurrentToken();
					break;
				case SymbolKind.Error:
					return ParseMessage.LexicalError;
				default:
					LalrAction action = currentState.GetActionBySymbol(inputToken.Symbol);
					if (action == null) {
						return ParseMessage.SyntaxError;
					}
					// the Execute() is the ParseToken() equivalent
					switch (action.Execute(this, inputToken)) {
					case TokenParseResult.Accept:
						return ParseMessage.Accept;
					case TokenParseResult.Shift:
						ClearCurrentToken();
						break;
					case TokenParseResult.SyntaxError:
						return ParseMessage.SyntaxError;
					case TokenParseResult.ReduceNormal:
						return ParseMessage.Reduction;
					case TokenParseResult.InternalError:
						return ParseMessage.InternalError;
					}
					break;
				}
			}
		}

		protected abstract bool CanTrim(Rule rule);

		protected abstract T CreateReduction(Rule rule, ReadOnlyCollection<T> children);

		private void ClearCurrentToken() {
			currentToken = default(T);
			currentTokenValid = false;
		}

		bool IParser<T>.CanTrim(Rule rule) {
			return CanTrim(rule);
		}

		T IParser<T>.PopToken() {
			return tokenStack.Pop().Key;
		}

		void IParser<T>.PushTokenAndState(T token, LalrState state) {
			Debug.Assert(state != null);
			tokenStack.Push(new KeyValuePair<T, LalrState>(token, state));
			currentState = state;
		}

		T IParser<T>.CreateReduction(Rule rule) {
			Debug.Assert(rule != null);
			T[] tokens = new T[rule.SymbolCount];
			for (int i = tokens.Length-1; i >= 0; i--) {
				tokens[i] = tokenStack.Pop().Key;
			}
			return CreateReduction(rule, Array.AsReadOnly(tokens));
		}

		LalrState IParser<T>.TopState {
			get {
				return tokenStack.Peek().Value;
			}
		}
	}

	///<summary>
	/// A concrete implementation of the <see cref="LalrProcessor{T}"/> using <see cref="Token"/> as token types
	///</summary>
	public class LalrProcessor: LalrProcessor<Token> {
		private readonly bool trim;

		/// <summary>
		/// Initializes a new instance of the <see cref="LalrProcessor"/> class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		public LalrProcessor(ITokenizer<Token> tokenizer): this(tokenizer, false) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="LalrProcessor"/> class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		/// <param name="trim">Trim the rules withn only one nonterminal away if set to <c>true</c>.</param>
		public LalrProcessor(ITokenizer<Token> tokenizer, bool trim): base(tokenizer) {
			this.trim = trim;
		}

		/// <summary>
		/// Creates the reduction.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="children">The children.</param>
		/// <returns></returns>
		protected override Token CreateReduction(Rule rule, ReadOnlyCollection<Token> children) {
			return new Reduction(rule, children);
		}

		protected override bool CanTrim(Rule rule) {
			return trim;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="LalrProcessor"/> does automatically trim the tokens with a single terminal.
		/// </summary>
		/// <value><c>true</c> if automatic trimming is enabled; otherwise, <c>false</c>.</value>
		public bool Trim {
			get {
				return trim;
			}
		}
	}
}