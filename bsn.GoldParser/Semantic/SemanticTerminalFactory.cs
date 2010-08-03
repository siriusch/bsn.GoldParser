using System;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// The abstract nongeneric case class for semantic terminal tokens. This class is for internal use only.
	/// </summary>
	public abstract class SemanticTerminalFactory: SemanticTokenFactory {
		internal SemanticTerminalFactory() {
		}

		internal abstract SemanticToken CreateInternal(string text);
	}
	
	/// <summary>
	/// The abstract generic case class for semantic terminal tokens. This class is usually not directly inherited.
	/// </summary>
	/// <typeparam name="T">The type of the terminal token.</typeparam>
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