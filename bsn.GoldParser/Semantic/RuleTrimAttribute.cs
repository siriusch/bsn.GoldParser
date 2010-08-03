using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// This attribute is used to define specific trim rules.
	/// </summary>
	/// <seealso cref="TerminalAttribute"/>
	/// <seealso cref="RuleAttribute"/>
	[AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class RuleTrimAttribute: RuleAttributeBase {
		private delegate ArgumentException ArgumentExceptionDelegate();

		private static ArgumentException SymbolNotFoundException() {
			return new ArgumentException("The specified symbol is not part of the rule", "trimSymbol");
		}

		private static ArgumentOutOfRangeException SymbolIndexOutOfRangeException() {
			return new ArgumentOutOfRangeException("trimSymbolIndex");
		}

		private readonly int trimSymbolIndex;
		private Type semanticTokenType;

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleTrimAttribute"/> class.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="trimSymbol">The trim symbol name.</param>
		public RuleTrimAttribute(string rule, string trimSymbol): this(rule, -1, trimSymbol, SymbolNotFoundException) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleTrimAttribute"/> class.
		/// </summary>
		/// <param name="rule">The rule.</param>
		/// <param name="trimSymbolIndex">Index of the trim symbol.</param>
		public RuleTrimAttribute(string rule, int trimSymbolIndex): this(rule, trimSymbolIndex, string.Empty, SymbolIndexOutOfRangeException) {}

		private RuleTrimAttribute(string rule, int trimSymbolIndex, string trimSymbol, ArgumentExceptionDelegate notFoundException): base(rule) {
			if (trimSymbol == null) {
				throw new ArgumentNullException("trimSymbol");
			}
			int ruleHandleCount = 0;
			using (IEnumerator<string> ruleHandleEnumerator = RuleDeclarationParser.GetRuleHandleNames(ParsedRule).GetEnumerator()) {
				while (ruleHandleEnumerator.MoveNext()) {
					if (trimSymbol.Equals(ruleHandleEnumerator.Current)) {
						if (trimSymbolIndex >= 0) {
							throw new ArgumentException("The given symbol is ambigious in this rule", "trimSymbol");
						}
						trimSymbolIndex = ruleHandleCount;
					}
					ruleHandleCount++;
				}
			}
			if ((trimSymbolIndex < 0) || (trimSymbolIndex >= ruleHandleCount)) {
				throw notFoundException();
			}
			this.trimSymbolIndex = trimSymbolIndex;
		}

		/// <summary>
		/// Gets or sets the type of the semantic token for which the trim rule has been defined.
		/// </summary>
		/// <value>The type of the semantic token.</value>
		public Type SemanticTokenType {
			get {
				return semanticTokenType;
			}
			set {
				semanticTokenType = value;
			}
		}

		/// <summary>
		/// Gets the handle index of the trim symbol.
		/// </summary>
		/// <value>The handle index of the trim symbol.</value>
		public int TrimSymbolIndex {
			get {
				return trimSymbolIndex;
			}
		}
	}
}