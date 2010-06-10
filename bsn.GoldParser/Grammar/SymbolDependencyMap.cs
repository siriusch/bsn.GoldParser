using System;
using System.Collections;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Grammar {
	public class SymbolDependencyMap: IComparer<Symbol> {
		private readonly Dictionary<Symbol, SymbolSet> symbolDependencies = new Dictionary<Symbol, SymbolSet>();

		public void AddDependecy(Symbol symbol, Symbol dependsOnSymbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (dependsOnSymbol == null) {
				throw new ArgumentNullException("dependsOnSymbol");
			}
			if (symbol == dependsOnSymbol) {
				throw new InvalidOperationException("Cannot add a dependency to itself");
			}
			if (DependsOn(dependsOnSymbol, symbol)) {
				throw new InvalidOperationException("Circular dependency detected");
			}
			SymbolSet dependencies;
			if (!symbolDependencies.TryGetValue(symbol, out dependencies)) {
				dependencies = new SymbolSet();
				symbolDependencies.Add(symbol, dependencies);
			}
			dependencies[dependsOnSymbol] = true;
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
			SymbolSet dependencies;
			if (symbolDependencies.TryGetValue(symbol, out dependencies)) {
				SymbolSet visited = new SymbolSet();
				visited[symbol] = true;
				Queue<Symbol> toCheck = new Queue<Symbol>(dependencies);
				while (toCheck.Count > 0) {
					Symbol symbolToCheck = toCheck.Dequeue();
					if (!visited[symbolToCheck]) {
						if (symbolToCheck == dependsOnSymbol) {
							return true;
						}
						if (symbolDependencies.TryGetValue(symbolToCheck, out dependencies)) {
							foreach (Symbol dependency in dependencies) {
								toCheck.Enqueue(dependency);
							}
						}
						visited[symbolToCheck] = true;
					}
				}
			}
			return false;
		}

		public int Compare(Symbol x, Symbol y) {
			if (DependsOn(x, y)) {
				return 1;
			}
			if (DependsOn(y, x)) {
				return -1;
			}
			return x.Index-y.Index;
		}
	}
}
