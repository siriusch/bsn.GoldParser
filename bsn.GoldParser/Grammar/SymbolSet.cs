using System.Collections;
using System.Collections.Generic;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// A set class for symbols.
	/// </summary>
	/// <remarks>
	/// This class is being used because there is no <c>HashSet&lt;&gt;</c> class in the version 2 of the framework.
	/// </remarks>
	public class SymbolSet: IEnumerable<Symbol> {
		private readonly Dictionary<Symbol, bool> entries = new Dictionary<Symbol, bool>();

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolSet"/> class.
		/// </summary>
		public SymbolSet() {}

		/// <summary>
		/// Includes or excludes the specified symbol from the set.
		/// </summary>
		public bool this[Symbol symbol] {
			get {
				bool result;
				return (symbol != null) && entries.TryGetValue(symbol, out result) && result;
			}
			set {
				if (value || entries.ContainsKey(symbol)) {
					entries[symbol] = value;
				}
			}
		}

		/// <summary>
		/// Sets the specified symbol.
		/// </summary>
		/// <param name="symbol">The symbol to be included.</param>
		/// <returns><c>true</c> if the symbol was not yet set.</returns>
		public bool Set(Symbol symbol) {
			bool isSet;
			if (entries.TryGetValue(symbol, out isSet)) {
				if (!isSet) {
					entries[symbol] = true;
				}
				return !isSet;
			}
			entries.Add(symbol, true);
			return true;
		}

		public IEnumerator<Symbol> GetEnumerator() {
			foreach (KeyValuePair<Symbol, bool> entry in entries) {
				if (entry.Value) {
					yield return entry.Key;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}