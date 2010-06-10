using System;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
	public sealed class RuleAttribute: RuleAttributeBase {
		public RuleAttribute(string rule): base(rule) {}
	}
}