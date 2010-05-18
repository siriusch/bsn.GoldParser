using System;
using System.Collections.Generic;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticActions<T> where T: IToken {
		private readonly CompiledGrammar grammar;
		private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>(StringComparer.Ordinal);
		private readonly Dictionary<Symbol, ICollection<Rule>> rulesOfSymbol = new Dictionary<Symbol, ICollection<Rule>>();
		private readonly Dictionary<Rule, SemanticTokenFactory<T>> factories = new Dictionary<Rule, SemanticTokenFactory<T>>();
		private readonly Dictionary<Symbol, SemanticTokenConverter> converters = new Dictionary<Symbol, SemanticTokenConverter>();

		public SemanticActions(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			this.grammar = grammar;
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Symbol symbol = grammar.GetSymbol(i);
				switch (symbol.Kind) {
				case SymbolKind.Terminal:
				case SymbolKind.Nonterminal:
					symbols.Add(symbol.ToString(), symbol);
					break;
				}
			}
			for (int i = 0; i < grammar.RuleCount; i++) {
				Rule rule = grammar.GetRule(i);
				ICollection<Rule> rules;
				if (!rulesOfSymbol.TryGetValue(rule.RuleSymbol, out rules)) {
					rules = new List<Rule>(4);
					rulesOfSymbol.Add(rule.RuleSymbol, rules);
				}
				rules.Add(rule);
			}
		}

		public CompiledGrammar Grammar {
			get {
				return grammar;
			}
		}

		public Symbol FindSymbol(string symbolName) {
			return symbols[symbolName];
		}

		public Rule FindRule(string ruleName, params string[] ruleParts) {
			if (string.IsNullOrEmpty(ruleName)) {
				throw new ArgumentNullException("ruleName");
			}
			if (ruleParts == null) {
				throw new ArgumentNullException("ruleParts");
			}
			Symbol ruleSymbol = FindSymbol(ruleName);
			Symbol[] parts = Array.ConvertAll(ruleParts, input => FindSymbol(input));
			foreach (Rule rule in rulesOfSymbol[ruleSymbol]) {
				if (rule.SymbolCount == parts.Length) {
					if (rule.Matches(parts)) {
						return rule;
					}
				}
			}
			throw new ArgumentException("The specified rule does not exist in the grammar with these symbols", "ruleName");
		}

		public void RegisterSemanticTokenCreator(Rule rule, SemanticTokenFactory<T> creator) {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if (rule.Owner != grammar) {
				throw new ArgumentException("The rule was defined on another grammar", "rule");
			}
			if (creator == null) {
				throw new ArgumentNullException("creator");
			}
			factories.Add(rule, creator);
		}

		public void RegisterSemanticTokenConverter(Symbol symbol, SemanticTokenConverter converter) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != grammar) {
				throw new ArgumentException("The symbol was defined on another grammar", "symbol");
			}
			if (symbol.Kind != SymbolKind.Terminal) {
				throw new ArgumentException("Symbol conversions are only possible for terminals", "symbol");
			}
			if (converter == null) {
				throw new ArgumentNullException("creator");
			}
			converters.Add(symbol, converter);
		}

		public bool TryGetFactory(Rule rule, out SemanticTokenFactory<T> factory) {
			return factories.TryGetValue(rule, out factory);
		}

		public bool TryGetConverter(Symbol symbol, out SemanticTokenConverter converter) {
			return converters.TryGetValue(symbol, out converter);
		}

		public void AssertRulesHaveActions() {
			for (int i = 0; i < grammar.RuleCount; i++) {
				Rule rule = grammar.GetRule(i);
				if (rule.SymbolCount > 1) {
					if (!factories.ContainsKey(rule)) {
						throw new InvalidOperationException(string.Format("Semantic action is missing for rule {0}", rule));
					}
				}
			}
		}
	}
}
