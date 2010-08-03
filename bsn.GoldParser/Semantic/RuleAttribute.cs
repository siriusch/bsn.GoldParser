using System;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// <para>Specifies the binding of a specific rule to a constructor (therefore implying the type to be constructed).</para>
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// public class MyRule<T>: SemanticToken {
	///   [Rule("<X> ::= Y <X>)]
	///   public MyRule(MyTerminal y, MyRule x) { ... }
	/// } ]]></code>
	/// </example>
	/// <remarks>
	/// <para>You can apply this attribute to constructors only.</para>
	/// <para>The class which defines the constructor must inherit from the type used as generic type parameter of the <see cref="SemanticTypeActions{T}"/> and be located in the same assembly in order to be found and associated with the grammar rule.</para>
	/// <para>Use the <see cref="ConstructorParameterMapping" /> to map rule handles arbitrarily to constructor parameters.</para>
	/// <para>If the class containing the constructor is generic, you can specify the generic type(s) to use following the rule string.</para>
	/// </remarks>
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