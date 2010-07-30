using System;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// This attribute is used to decorate constructors on <see cref="SemanticToken"/> descendant classes for automatic binding of the specified grammar to types created.
	/// </summary>
	/// <seealso cref="TerminalAttribute"/>
	/// <seealso cref="RuleTrimAttribute"/>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple=true, Inherited=false)]
	public sealed class RuleAttribute: RuleAttributeBase, IEquatable<RuleAttribute> {
		private readonly Type[] genericTypeParameters;
		private bool allowTruncationForConstructor;
		private int[] constructorParameterMapping;

		/// <summary>
		/// Define that the constructor where the attribute is applied shall be invoked for the given reduction rule.
		/// </summary>
		/// <param name="rule">The rule (in the same form as in the grammar file, such as <c> &lt;List&gt; ::= Item ',' &lt;List&gt;</c>).</param>
		public RuleAttribute(string rule): base(rule) {}

		/// <summary>
		/// Define that the constructor where the attribute is applied shall be invoked on the closed generic type for the given reduction rule.
		/// </summary>
		/// <param name="rule">The rule (in the same form as in the grammar file, such as <c>&lt;List&gt; ::= Item ',' &lt;List&gt;</c>).</param>
		/// <param name="genericTypeParameters">The type parameters to use for closing the generic type.</param>
		public RuleAttribute(string rule, params Type[] genericTypeParameters)
			: this(rule) {
			this.genericTypeParameters = genericTypeParameters;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the list of symbols may be truncated when invoking the constructor.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>The same functionality can be achieved with an explicit <see cref="ConstructorParameterMapping"/>.</item>
		/// <item>If a <see cref="ConstructorParameterMapping"/> is defined, this property has no function.</item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <para>Rule: <c>&lt;List&gt; ::= Item ';'</c></para>
		/// <para>Constructor: <c>MyList(MyItem item)</c></para>
		/// <para>Assuming that there is no explicit <see cref="ConstructorParameterMapping"/> defined, <see cref="AllowTruncationForConstructor"/> must be <c>true</c> to pass the consistency check in this example, since the <c>';'</c> symbol is to be truncated.</para>
		/// </example>
		/// <value>
		/// 	<c>true</c> to allow truncation for the constructor, otherwise <c>false</c>.
		/// </value>
		public bool AllowTruncationForConstructor {
			get {
				return allowTruncationForConstructor;
			}
			set {
				allowTruncationForConstructor = value;
			}
		}

		/// <summary>
		/// Allows to define an explicit mapping of symbols to constructor parameters. The indices are 0-based.
		/// </summary>
		/// <value>An array with exacltly one integer index for each constructor parameter. The index must point to one of the symbols (0-based).</value>
		/// <example>
		/// Rule: <c>&lt;List&gt; ::= Item ',' &lt;List&gt;</c>
		/// Constructor: <c>MyList(MyList next, Item item)</c>
		/// Mapping: <c>new int[] {2, 0}</c>
		/// </example>
		public int[] ConstructorParameterMapping {
			get {
				return constructorParameterMapping;
			}
			set {
				constructorParameterMapping = value;
			}
		}

		/// <summary>
		/// Gets the generic type parameters.
		/// </summary>
		/// <value>The generic type parameters.</value>
		public Type[] GenericTypeParameters {
			get {
				return genericTypeParameters ?? Type.EmptyTypes;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has a constructor parameter mapping.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has a constructor parameter mapping; otherwise, <c>false</c>.
		/// </value>
		public bool HasConstructorParameterMapping {
			get {
				return constructorParameterMapping != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is instantiating a generic type.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is instantiating a generic type; otherwise, <c>false</c>.
		/// </value>
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