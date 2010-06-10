using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
	public sealed class RuleTrimAttribute: RuleAttributeBase {
		private readonly int trimSymbolIndex;

		public RuleTrimAttribute(string rule, int trimSymbolIndex) : base(rule) {
			int ruleHandleCount = 0;
			using (IEnumerator<string> ruleHandleEnumerator = RuleDeclarationParser.GetRuleHandleNames(ParsedRule).GetEnumerator()) {
				while (ruleHandleEnumerator.MoveNext()) {
					ruleHandleCount++;
				}
			}
			if ((trimSymbolIndex < 0) || (trimSymbolIndex >= ruleHandleCount)) {
				throw new ArgumentOutOfRangeException("trimSymbolIndex");
			}
			this.trimSymbolIndex = trimSymbolIndex;
		}

		public int TrimSymbolIndex {
			get {
				return trimSymbolIndex;
			}
		}
	}
}