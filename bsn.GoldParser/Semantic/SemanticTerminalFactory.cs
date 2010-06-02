using System;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticTerminalFactory {
		internal SemanticTerminalFactory() {
		}

		public abstract Type OutputType {
			get;
		}

		internal abstract SemanticToken CreateInternal(string text);
	}

	public abstract class SemanticTerminalFactory<T>: SemanticTerminalFactory where T: SemanticToken {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		protected abstract T Create(string text);

		internal override sealed SemanticToken CreateInternal(string text) {
			return Create(text);
		}
	}
}