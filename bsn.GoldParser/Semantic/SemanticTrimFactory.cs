using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticTrimFactory<T>: SemanticNonterminalFactory where T: SemanticToken {
		private readonly int handleIndex;
		private readonly SemanticActions<T> owner;
		private readonly Rule rule;

		internal SemanticTrimFactory(SemanticActions<T> owner, Rule rule, int handleIndex) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if ((handleIndex < 0) || (handleIndex >= rule.SymbolCount)) {
				throw new ArgumentOutOfRangeException("handleIndex");
			}
			this.owner = owner;
			this.handleIndex = handleIndex;
			this.rule = rule;
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return Array.AsReadOnly(new[] {GetRuleType()});
			}
		}

		public override Type OutputType {
			get {
				return GetRuleType();
			}
		}

		protected internal override Symbol RedirectForOutputType {
			get {
				return GetTrimSymbol();
			}
		}

		private Symbol GetTrimSymbol() {
			return rule[handleIndex];
		}

		protected internal override System.Collections.Generic.IEnumerable<Symbol> GetInputSymbols(Rule rule) {
			yield return GetTrimSymbol();
		}

		internal override SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			Debug.Assert(this.rule == rule);
			SemanticToken result = tokens[handleIndex];
			Debug.Assert(((IToken)result).Symbol == GetTrimSymbol());
			Debug.Assert(OutputType.IsAssignableFrom(result.GetType()));
			return result;
		}

		private Type GetRuleType() {
			if (owner.Initialized) {
				return owner.GetSymbolOutputType(GetTrimSymbol());
			}
			return typeof(T);
		}
	}
}
