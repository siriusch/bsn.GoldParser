using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticActions<T> where T: SemanticToken {
		private class SemanticTokenizer: Tokenizer<SemanticToken> {
			private readonly SemanticActions<T> actions;

			public SemanticTokenizer(TextReader textReader, SemanticActions<T> actions): base(textReader, actions.Grammar) {
				this.actions = actions;
			}

			protected override SemanticToken CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
				return actions.CreateTerminalToken(tokenSymbol, tokenPosition, text);
			}
		}

		internal static CompiledGrammar GetGrammar(SemanticActions<T> actions) {
			if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			return actions.Grammar;
		}

		private readonly Dictionary<Symbol, SemanticTerminalFactory> terminalFactories = new Dictionary<Symbol, SemanticTerminalFactory>();
		private readonly Dictionary<Rule, SemanticNonterminalFactory> nonterminalFactories = new Dictionary<Rule, SemanticNonterminalFactory>();

		private readonly CompiledGrammar grammar;

		public SemanticActions(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			this.grammar = grammar;
		}

		public CompiledGrammar Grammar {
			get {
				return grammar;
			}
		}

		protected void CheckConsistency() {
			Dictionary<Symbol, Type> symbolTypes = new Dictionary<Symbol, Type>();
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Symbol symbol = grammar.GetSymbol(i);
				SemanticTerminalFactory factory;
				if (!terminalFactories.TryGetValue(symbol, out factory)) {
					
				}
			}
			for (int i = 0; i < grammar.RuleCount; i++) {
				Rule rule = grammar.GetRule(i);
				if (rule.SymbolCount > 1) {
					if (!nonterminalFactories.ContainsKey(rule)) {
						throw new InvalidOperationException(string.Format("Semantic action is missing for rule {0}", rule));
					}
				}
			}
		}

		protected void RegisterTerminalFactory(Symbol symbol, SemanticTerminalFactory factory) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != grammar) {
				throw new ArgumentException("The symbol was defined on another grammar", "symbol");
			}
			if (symbol.Kind != SymbolKind.Terminal) {
				throw new ArgumentException("Symbol conversions are only possible for terminals", "symbol");
			}
			if (factory == null) {
				throw new ArgumentNullException("factory");
			}
			terminalFactories.Add(symbol, factory);
		}

		protected void RegisterNonterminalFactory(Rule rule, SemanticNonterminalFactory factory) {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if (rule.Owner != grammar) {
				throw new ArgumentException("The rule was defined on another grammar", "rule");
			}
			if (factory == null) {
				throw new ArgumentNullException("factory");
			}
			nonterminalFactories.Add(rule, factory);
		}

		public bool TryGetConverter(Symbol symbol, out SemanticTerminalFactory converter) {
			return terminalFactories.TryGetValue(symbol, out converter);
		}

		public bool TryGetFactory(Rule rule, out SemanticNonterminalFactory factory) {
			return nonterminalFactories.TryGetValue(rule, out factory);
		}

		protected internal virtual ITokenizer<SemanticToken> CreateTokenizer(TextReader reader) {
			return new SemanticTokenizer(reader, this);
		}

		private SemanticToken CreateTerminalToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
			SemanticToken result = terminalFactories[tokenSymbol].CreateInternal(text);
			result.Initialize(tokenSymbol, tokenPosition);
			return result;
		}
	}
}
