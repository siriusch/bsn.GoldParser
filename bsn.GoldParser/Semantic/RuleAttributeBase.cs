using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class RuleAttributeBase: Attribute {
		private readonly Reduction parsedRule;

		protected RuleAttributeBase(string rule) {
			if (string.IsNullOrEmpty(rule)) {
				throw new ArgumentNullException("rule");
			}
			if (!RuleDeclarationParser.TryParse(rule, out parsedRule)) {
				throw new ArgumentException("The given rule contains a syntax error", "rule");
			}
		}

		public string Rule {
			get {
				return parsedRule.ToString();
			}
		}

		internal Reduction ParsedRule {
			get {
				return parsedRule;
			}
		}

		public Rule Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Rule rule;
			RuleDeclarationParser.TryBind(parsedRule, grammar, out rule);
			return rule;
		}
	}
}