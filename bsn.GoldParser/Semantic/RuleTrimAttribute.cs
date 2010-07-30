using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Semantic {
	[AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public sealed class RuleTrimAttribute: RuleAttributeBase {
		private delegate ArgumentException ArgumentExceptionDelegate();

		private readonly int trimSymbolIndex;
		private Type semanticTokenType;

		public RuleTrimAttribute(string rule, string trimSymbol)
			: this(rule, -1, trimSymbol, SymbolNotFoundException) {
		}

		public Type SemanticTokenType {
			get {
				return semanticTokenType;
			}
			set {
				semanticTokenType = value;
			}
		}

		private static ArgumentException SymbolNotFoundException() {
			return new ArgumentException("The specified symbol is not part of the rule", "trimSymbol");
		}

		public RuleTrimAttribute(string rule, int trimSymbolIndex) : this(rule, trimSymbolIndex, string.Empty, SymbolIndexOutOfRangeException) {}

		private static ArgumentOutOfRangeException SymbolIndexOutOfRangeException() {
			return new ArgumentOutOfRangeException("trimSymbolIndex");
		}

		private RuleTrimAttribute(string rule, int trimSymbolIndex, string trimSymbol, ArgumentExceptionDelegate notFoundException)
			: base(rule) {
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

		public int TrimSymbolIndex {
			get {
				return trimSymbolIndex;
			}
		}
	}
}