using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class RuleAttribute: Attribute {
		private readonly Reduction parsedRule;

		public RuleAttribute(string rule) {
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

		public Rule Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Rule rule;
			if (!RuleDeclarationParser.TryBind(parsedRule, grammar, out rule)) {
				throw new InvalidOperationException(string.Format("The rule {0} cannot be bound to the grammar {1}", parsedRule, grammar));
			}
			return rule;
		}
	}
}