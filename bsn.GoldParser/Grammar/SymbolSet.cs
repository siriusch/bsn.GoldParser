// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-goldparser.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
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