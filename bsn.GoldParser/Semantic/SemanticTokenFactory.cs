using System;
using System.Collections.Generic;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenFactory {
		internal SemanticTokenFactory() {}

		public abstract ICollection<Type> InputTypes {
			get;
		}

		public abstract Type OutputType {
			get;
		}

		internal abstract Token CreateInternal(Rule rule, IToken[] tokens);
	}

	public abstract class SemanticTokenFactory<T>: SemanticTokenFactory where T: Token {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		protected abstract T Create(Rule rule, IToken[] tokens);

		internal override sealed Token CreateInternal(Rule rule, IToken[] tokens) {
			return Create(rule, tokens);
		}
	}
}