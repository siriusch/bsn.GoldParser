using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class RuleAttributeBase: Attribute {
		private Reduction parsedRule;

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

		public Rule Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Rule rule;
			RuleDeclarationParser.TryBind(parsedRule, grammar, out rule);
			return rule;
		}
	}

	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
	public sealed class RuleAttribute: RuleAttributeBase {
		public RuleAttribute(string rule): base(rule) {}
	}

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
	public sealed class RuleTrimAttribute: RuleAttributeBase {
		private readonly int indexOfSymbolToKeep;

		public RuleTrimAttribute(string rule, int indexOfSymbolToKeep) : base(rule) {
			this.indexOfSymbolToKeep = indexOfSymbolToKeep;
		}

		public int IndexOfSymbolToKeep {
			get {
				return indexOfSymbolToKeep;
			}
		}
	}
}