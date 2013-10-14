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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Pull parser which uses Grammar table to parse input tokens.
	/// </summary>
	/// <remarks>This class can be used for further spezialization with custom tokenizers.</remarks>
	public abstract class LalrProcessor<TToken, TTokenizer>: IParser<TToken> where TToken: class, IToken
			where TTokenizer: class, ITokenizer<TToken> {
		private readonly Queue<TToken> tokenInject = new Queue<TToken>(); // Tokens to inject
		private readonly LalrStack<TToken> tokenStack; // Stack of LR states used for LR parsing.
		private readonly TTokenizer tokenizer;
		private LalrState currentState;
		private TToken currentToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="LalrProcessor{TTokenizer}" /> class.
		/// </summary>
		/// <param name="tokenizer">The tokenizer.</param>
		/// <exception cref="System.ArgumentNullException">tokenizer</exception>
		protected LalrProcessor(TTokenizer tokenizer) {
			if (tokenizer == null) {
				throw new ArgumentNullException("tokenizer");
			}
			this.tokenizer = tokenizer;
			currentState = tokenizer.Grammar.InitialLRState;
			tokenStack = new LalrStack<TToken>(currentState);
		}

		/// <summary>
		/// Gets the current currentToken.
		/// </summary>
		/// <value>The current currentToken.</value>
		public TToken CurrentToken {
			get {
				if (currentToken != null) {
					return currentToken;
				}
				return tokenStack.Peek();
			}
		}

		protected TTokenizer Tokenizer {
			get {
				return tokenizer;
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
		public virtual ParseMessage Parse() {
			while (true) {
				TToken inputToken;
				if (currentToken == null) {
					//We must read a currentToken
					TToken textInputToken;
					ParseMessage message;
					if (tokenInject.Count > 0) {
						textInputToken = tokenInject.Dequeue();
						message = ParseMessage.TokenRead;
					} else {
						message = tokenizer.NextToken(out textInputToken);
						if (textInputToken == null) {
							return ParseMessage.InternalError;
						}
					}
					//					Debug.WriteLine(string.Format("State: {0} Line: {1}, Column: {2}, Parse Value: {3}, Token Type: {4}", currentState.Index, inputToken.Line, inputToken.LinePosition, inputToken.Text, inputToken.symbol.Name), "Token Read");
					switch (textInputToken.Symbol.Kind) {
					case SymbolKind.End:
					case SymbolKind.Error:
						inputToken = textInputToken;
						break;
					default:
						currentToken = textInputToken;
						return message;
					}
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
					if (RetryLexicalError(ref inputToken)) {
						currentToken = inputToken;
						continue;
					}
					return ParseMessage.LexicalError;
				default:
					LalrAction action = currentState.GetActionBySymbol(inputToken.Symbol);
					if (action == null) {
						if (RetrySyntaxError(ref inputToken)) {
							currentToken = inputToken;
							continue;
						}
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

		public ParseMessage ParseAll() {
			ParseMessage result;
			do {
				result = Parse();
			} while (CompiledGrammar.CanContinueParsing(result));
			return result;
		}

		protected abstract bool CanTrim(Rule rule);

		protected abstract TToken CreateReduction(Rule rule, IList<TToken> children);

		protected void Inject(TToken token) {
			if (token == null) {
				throw new ArgumentNullException("token");
			}
			tokenInject.Enqueue(token);
		}

		protected virtual bool RetryLexicalError(ref TToken currentToken) {
			return false;
		}

		protected virtual bool RetrySyntaxError(ref TToken currentToken) {
			return false;
		}

		private void ClearCurrentToken() {
			currentToken = default(TToken);
		}

		bool IParser<TToken>.CanTrim(Rule rule) {
			return CanTrim(rule);
		}

		TToken IParser<TToken>.PopToken() {
			return tokenStack.Pop();
		}

		void IParser<TToken>.PushTokenAndState(TToken token, LalrState state) {
			Debug.Assert(state != null);
			tokenStack.Push(token, state);
			currentState = state;
		}

		TToken IParser<TToken>.CreateReduction(Rule rule) {
			Debug.Assert(rule != null);
			return CreateReduction(rule, tokenStack.PopRange(rule.SymbolCount));
		}

		LalrState IParser<TToken>.TopState {
			get {
				return tokenStack.GetTopState();
			}
		}
	}

	/// <summary>
	/// Pull parser which uses Grammar table to parse input stream.
	/// </summary>
	public abstract class LalrProcessor<T>: LalrProcessor<T, ITokenizer<T>> where T: class, IToken {
		protected LalrProcessor(ITokenizer<T> tokenizer): base(tokenizer) {}
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
		/// Gets a value indicating whether this <see cref="LalrProcessor"/> does automatically trim the tokens with a single terminal.
		/// </summary>
		/// <value><c>true</c> if automatic trimming is enabled; otherwise, <c>false</c>.</value>
		public bool Trim {
			get {
				return trim;
			}
		}

		protected override bool CanTrim(Rule rule) {
			return trim;
		}

		/// <summary>
		/// Creates the reduction.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="children">The children.</param>
		/// <returns></returns>
		protected override Token CreateReduction(Rule rule, IList<Token> children) {
			return new Reduction(rule, children);
		}
	}
}
