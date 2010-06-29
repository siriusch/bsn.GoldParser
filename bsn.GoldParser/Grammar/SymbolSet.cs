using System.Collections;
using System.Collections.Generic;

namespace bsn.GoldParser.Grammar {
	public class SymbolSet: IEnumerable<Symbol> {
		private readonly Dictionary<Symbol, bool> entries = new Dictionary<Symbol, bool>();

		public SymbolSet() {}

		public bool this[Symbol symbol] {
			get {
				bool result;
				return entries.TryGetValue(symbol, out result) && result;
			}
			set {
				if (value || entries.ContainsKey(symbol)) {
					entries[symbol] = value;
				}
			}
		}

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