using System;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
	public sealed class RuleAttribute: RuleAttributeBase, IEquatable<RuleAttribute> {
		private readonly Type[] genericTypes;
		private bool allowTruncation;
		private int[] parameterMapping;

		public RuleAttribute(string rule): this(rule, null) {}

		public RuleAttribute(string rule, params Type[] genericTypes): base(rule) {
			this.genericTypes = genericTypes ?? Type.EmptyTypes;
		}

		public Type[] GenericTypes {
			get {
				return genericTypes;
			}
		}

		public bool IsGeneric {
			get {
				return genericTypes.Length > 0;
			}
		}

		public bool AllowTruncation {
			get {
				return allowTruncation;
			}
			set {
				allowTruncation = value;
			}
		}

		public int[] ParameterMapping {
			get {
				return parameterMapping;
			}
			set {
				parameterMapping = value;
			}
		}

		public override bool Equals(object obj) {
			return base.Equals(obj as RuleAttribute);
		}

		public override int GetHashCode() {
			return ParsedRule.ToString().GetHashCode();
		}

		public bool Equals(RuleAttribute other) {
			return (other != null) && ParsedRule.ToString().Equals(other.ParsedRule.ToString(), StringComparison.Ordinal);
		}
	}
}