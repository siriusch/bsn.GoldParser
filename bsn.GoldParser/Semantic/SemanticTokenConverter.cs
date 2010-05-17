using System;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTokenConverter<T>: SemanticTokenConverter where T: Token {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		protected abstract T Create(TextToken token);

		internal override sealed Token ConvertInternal(TextToken token) {
			return Create(token);
		}
	}

	public abstract class SemanticTokenConverter {
		internal SemanticTokenConverter() {}

		public abstract Type OutputType {
			get;
		}

		internal abstract Token ConvertInternal(TextToken token);
	}
}