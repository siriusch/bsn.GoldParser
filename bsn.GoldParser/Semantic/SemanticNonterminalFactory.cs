using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticNonterminalFactory: SemanticTokenFactory {
		public abstract ReadOnlyCollection<Type> InputTypes {
			get;
		}

		internal abstract SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens);
	}

	public abstract class SemanticNonterminalFactory<T>: SemanticNonterminalFactory where T: SemanticToken {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		public abstract T Create(Rule rule, ReadOnlyCollection<SemanticToken> tokens);

		internal override sealed SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			return Create(rule, tokens);
		}

		public sealed override bool IsStaticOutputType {
			get {
				return true;
			}
		}
	}
}