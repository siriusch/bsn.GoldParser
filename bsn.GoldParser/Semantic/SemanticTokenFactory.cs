using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenFactory<T> where T: IToken {
		public abstract ICollection<Type> InputTypes {
			get;
		}

		public Type OutputType {
			get {
				return typeof(T);
			}
		}

		protected abstract T Create(Rule rule, ReadOnlyCollection<T> tokens);

		internal T CreateInternal(Rule rule, ReadOnlyCollection<T> tokens) {
			return Create(rule, tokens);
		}
	}
}