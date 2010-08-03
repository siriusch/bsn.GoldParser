using System;
using System.Diagnostics;
using System.Reflection;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// The factory for terminals of the semantic token type implementation.
	/// </summary>
	/// <typeparam name="T">The <see cref="SemanticToken"/> descendant instantiated by this factory.</typeparam>
	public class SemanticTerminalTypeFactory<T>: SemanticTerminalFactory<T> where T: SemanticToken {
		private readonly SemanticTerminalTypeFactoryHelper.Activator<T> activator;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemanticTerminalTypeFactory&lt;T&gt;"/> class. This is mainly for internal use.
		/// </summary>
		public SemanticTerminalTypeFactory() {
			ConstructorInfo constructor = typeof(T).GetConstructor(new[] {typeof(string)});
			if (constructor == null) {
				constructor = typeof(T).GetConstructor(Type.EmptyTypes);
				if (constructor == null) {
					throw new InvalidOperationException("No matching constructor found");
				}
			}
			activator = SemanticTerminalTypeFactoryHelper.CreateActivator(this, constructor);
			Debug.Assert(activator != null);
		}

		protected override T Create(string text) {
			return activator(text);
		}
	}
}
