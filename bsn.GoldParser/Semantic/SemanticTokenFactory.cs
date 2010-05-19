using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenFactory {
		public abstract ICollection<Type> InputTypes {
			get;
		}

		public abstract Type OutputType {
			get;
		}

		internal abstract SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens);
	}

	public abstract class SemanticTokenFactory<T>: SemanticTokenFactory where T: SemanticToken {
		public sealed override Type OutputType {
			get {
				return typeof(T);
			}
		}

		public abstract T Create(Rule rule, ReadOnlyCollection<SemanticToken> tokens);

		internal sealed override SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			return Create(rule, tokens);
		}
	}
}
