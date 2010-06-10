using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Grammar {
	public class SymbolDependencyMap: IComparer<Symbol> {
		private readonly Dictionary<Symbol, SymbolSet> symbolDependencies = new Dictionary<Symbol, SymbolSet>();
		public ICollection<Symbol> SymbolsWithDependencies {
			get {
				return symbolDependencies.Keys;
			}
		}

		public void AddDependecy(Symbol symbol, Symbol dependsOnSymbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (dependsOnSymbol == null) {
				throw new ArgumentNullException("dependsOnSymbol");
			}
			if (symbol != dependsOnSymbol) {
				SymbolSet dependencies;
				if (!symbolDependencies.TryGetValue(symbol, out dependencies)) {
					dependencies = new SymbolSet();
					symbolDependencies.Add(symbol, dependencies);
				}
				dependencies[dependsOnSymbol] = true;
			}
		}

		public bool DependsOn(Symbol symbol, Symbol dependsOnSymbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (dependsOnSymbol == null) {
				throw new ArgumentNullException("dependsOnSymbol");
			}
			if (symbol == dependsOnSymbol) {
				return false;
			}
			foreach (Symbol dependency in GetDependencies(symbol)) {
				if (dependency == dependsOnSymbol) {
					return true;
				}
			}
			return false;
		}

		public IEnumerable<Symbol> GetDependencies(Symbol symbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			SymbolSet dependencies;
			if (symbolDependencies.TryGetValue(symbol, out dependencies)) {
				SymbolSet visited = new SymbolSet();
				visited[symbol] = true;
				Queue<Symbol> toCheck = new Queue<Symbol>(dependencies);
				while (toCheck.Count > 0) {
					Symbol symbolToCheck = toCheck.Dequeue();
					if (!visited[symbolToCheck]) {
						yield return symbolToCheck;
						if (symbolDependencies.TryGetValue(symbolToCheck, out dependencies)) {
							foreach (Symbol dependency in dependencies) {
								toCheck.Enqueue(dependency);
							}
						}
						visited[symbolToCheck] = true;
					}
				}
			}
		}

		public int Compare(Symbol x, Symbol y) {
			// in order to ensure that x>y == y<x, we need to compare both directions
			bool xOnY = DependsOn(x, y);
			if (xOnY != DependsOn(y, x)) {
				if (xOnY) {
					return 1;
				}
				return -1;
			}
			// either no dependency or a circular dependency, just compare the index in that case
			return x.Index-y.Index;
		}
	}
}
