using System;
using System.Reflection;

namespace bsn.GoldParser.Semantic {
	public class SemanticTerminalTypeFactory<T>: SemanticTerminalFactory<T> where T: SemanticToken {
#warning replace reflection with generated IL code
		private readonly ConstructorInfo constructor;
		private readonly bool defaultConstructor;

		public SemanticTerminalTypeFactory() {
			constructor = typeof(T).GetConstructor(new[] {typeof(string)});
			if (constructor == null) {
				constructor = typeof(T).GetConstructor(Type.EmptyTypes);
				if (constructor == null) {
					throw new InvalidOperationException("No matching constructor found");
				}
				defaultConstructor = true;
			}
		}

		protected override T Create(string text) {
			object[] args = null;
			if (!defaultConstructor) {
				args = new[] {text};
			}
			return (T)constructor.Invoke(args);
		}
	}
}