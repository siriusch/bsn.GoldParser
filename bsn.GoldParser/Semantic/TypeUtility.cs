using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	internal class TypeUtility<T> where T: SemanticToken {
		private readonly Dictionary<Type, ReadOnlyCollection<Type>> baseTypeCache = new Dictionary<Type, ReadOnlyCollection<Type>>();
		private readonly Dictionary<Symbol, Type> symbolType = new Dictionary<Symbol, Type>();

		public TypeUtility() {}

		public ReadOnlyCollection<Type> GetBaseTypes(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (!typeof(T).IsAssignableFrom(type)) {
				throw new ArgumentException("Not an allowed base type", "type");
			}
			ReadOnlyCollection<Type> result;
			if (!baseTypeCache.TryGetValue(type, out result)) {
				List<Type> ancestorOrSelf = new List<Type>();
				Type ancestor = type;
				while (ancestor != typeof(T)) {
					ancestor = ancestor.BaseType;
					ancestorOrSelf.Add(ancestor);
				}
				ancestorOrSelf.Reverse();
				result = ancestorOrSelf.AsReadOnly();
				baseTypeCache.Add(type, result);
			}
			return result;
		}

		public Type GetCommonBaseType(Type x, Type y) {
			ReadOnlyCollection<Type> xBase = GetBaseTypes(x);
			ReadOnlyCollection<Type> yBase = GetBaseTypes(y);
			Type result = typeof(T);
			for (int i = 0; (i < xBase.Count) && (i < yBase.Count); i++) {
				if (xBase[i] != yBase[i]) {
					break;
				}
				result = xBase[i];
			}
			return result;
		}

		public Type GetSymbolType(Symbol symbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			Type result;
			if (symbolType.TryGetValue(symbol, out result)) {
				return result;
			}
			return typeof(T);
		}

		public void MemorizeTypeForSymbol(Symbol symbol, Type type) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			Type currentType;
			if (symbolType.TryGetValue(symbol, out currentType)) {
				symbolType[symbol] = GetCommonBaseType(currentType, type);
			} else {
				symbolType.Add(symbol, type);
			}
		}
	}
}
