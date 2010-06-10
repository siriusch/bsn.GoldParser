using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
	public sealed class RuleTrimAttribute: RuleAttributeBase {
		private readonly int indexOfSymbolToKeep;

		public RuleTrimAttribute(string rule, int indexOfSymbolToKeep) : base(rule) {
			int ruleHandleCount = 0;
			using (IEnumerator<string> ruleHandleEnumerator = RuleDeclarationParser.GetRuleHandleNames(ParsedRule).GetEnumerator()) {
				while (ruleHandleEnumerator.MoveNext()) {
					ruleHandleCount++;
				}
			}
			if ((indexOfSymbolToKeep < 0) || (indexOfSymbolToKeep >= ruleHandleCount)) {
				throw new ArgumentOutOfRangeException("indexOfSymbolToKeep");
			}
			this.indexOfSymbolToKeep = indexOfSymbolToKeep;
		}

		public int IndexOfSymbolToKeep {
			get {
				return indexOfSymbolToKeep;
			}
		}
	}
}