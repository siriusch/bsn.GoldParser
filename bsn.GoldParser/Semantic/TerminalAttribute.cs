using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// This class is used to decorate constructors which accept exactly one string for the terminal value
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = true)]
	public sealed class TerminalAttribute: Attribute {
		private readonly string symbolName;

		public TerminalAttribute(string symbolName) {
			if (string.IsNullOrEmpty(symbolName)) {
				throw new ArgumentNullException("symbolName");
			}
			this.symbolName = symbolName;
		}

		public Symbol Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Symbol result;
			grammar.TryGetSymbol(symbolName, out result);
			return result;
		}

		public string SymbolName {
			get {
				return symbolName;
			}
		}
	}
}
