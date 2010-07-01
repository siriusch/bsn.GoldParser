using System;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
	public sealed class RuleAttribute: RuleAttributeBase, IEquatable<RuleAttribute> {
		private readonly Type[] genericTypeParameters;
		private bool allowTruncationForConstructor;
		private int[] constructorParameterMapping;

		public RuleAttribute(string rule): base(rule) {}

		public RuleAttribute(string rule, params Type[] genericTypeParameters): this(rule) {
			this.genericTypeParameters = genericTypeParameters;
		}

		public bool AllowTruncationForConstructor {
			get {
				return allowTruncationForConstructor;
			}
			set {
				allowTruncationForConstructor = value;
			}
		}

		public int[] ConstructorParameterMapping {
			get {
				return constructorParameterMapping;
			}
			set {
				constructorParameterMapping = value;
			}
		}

		public Type[] GenericTypeParameters {
			get {
				return genericTypeParameters ?? Type.EmptyTypes;
			}
		}

		public bool HasConstructorParameterMapping {
			get {
				return constructorParameterMapping != null;
			}
		}

		public bool IsGeneric {
			get {
				return (genericTypeParameters != null) && (genericTypeParameters.Length > 0);
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