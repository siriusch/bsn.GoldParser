using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	internal class SymbolTypeMap<T> where T: SemanticToken {
		private readonly Dictionary<Type, ReadOnlyCollection<Type>> baseTypeCache;
		private readonly Dictionary<Symbol, Type> symbolType = new Dictionary<Symbol, Type>();
		private readonly SymbolTypeMap<T> parent;
		private int version;

		public SymbolTypeMap(SymbolTypeMap<T> parent) {
			this.parent = parent;
			if (parent == null) {
				baseTypeCache = new Dictionary<Type, ReadOnlyCollection<Type>>();
			}
		}

		public SymbolTypeMap(): this(null) {}

		public ReadOnlyCollection<Type> GetBaseTypes(Type type) {
			if (parent != null) {
				return parent.GetBaseTypes(type);
			}
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (!typeof(T).IsAssignableFrom(type)) {
				throw new ArgumentException("Not an allowed base type", "type");
			}
			ReadOnlyCollection<Type> result;
			lock (baseTypeCache) {
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

		internal void ApplyCommonBaseType(ref Type x, Type y) {
			Debug.Assert(y != null);
			x = (x == null) ? y : GetCommonBaseType(x, y);
		}

		protected Type GetSymbolTypeInternal(Symbol symbol, Type @default) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			Type result;
			symbolType.TryGetValue(symbol, out result);
			if (parent != null) {
				Type parentResult = parent.GetSymbolTypeInternal(symbol, null);
				if (parentResult != null) {
					if (result != null) {
						return GetCommonBaseType(result, parentResult);
					}
					return parentResult;
				}
			}
			return result ?? @default;
		}

		public Type GetSymbolType(Symbol symbol) {
			return GetSymbolTypeInternal(symbol, typeof(T));
		}

		public void SetTypeForSymbol(Symbol symbol, Type type) {
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
			version++;
		}

		public int Version {
			get {
				if (parent != null) {
					return parent.Version+version;
				}
				return version;
			}
		}
	}
}
