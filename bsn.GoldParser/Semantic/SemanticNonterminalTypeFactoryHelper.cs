using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.GoldParser.Semantic {
	internal static class SemanticNonterminalTypeFactoryHelper {
		public delegate T Activator<T>(ReadOnlyCollection<SemanticToken> tokens);

		private static readonly Dictionary<ConstructorInfo, DynamicMethod> dynamicMethods = new Dictionary<ConstructorInfo, DynamicMethod>();
		private static readonly MethodInfo readOnlyCollectionGetItem = GetReadOnlyCollectionGetItemMethod();

		private static MethodInfo GetReadOnlyCollectionGetItemMethod() {
			MethodInfo result = typeof(ReadOnlyCollection<SemanticToken>).GetProperty("Item").GetGetMethod();
			Debug.Assert(result != null);
			return result;
		}

		private static DynamicMethod GetDynamicMethod(ConstructorInfo constructor) {
			Debug.Assert(constructor != null);
			lock (dynamicMethods) {
				DynamicMethod result;
				if (!dynamicMethods.TryGetValue(constructor, out result)) {
					result = new DynamicMethod(string.Format("SemanticNonterminalTypeFactory<{0}>.Activator", constructor.DeclaringType.FullName), constructor.DeclaringType, new[] {typeof(int[]), typeof(ReadOnlyCollection<SemanticToken>)}, true);
					ILGenerator il = result.GetILGenerator();
					int parameterCount = constructor.GetParameters().Length;
					for (int i = 0; i < parameterCount; i++) {
						il.Emit(OpCodes.Ldarg_1); // load the ReadOnlyCollection<SemanticToken>
						il.Emit(OpCodes.Ldarg_0); // load the int[]
						il.Emit(OpCodes.Ldc_I4, i); // load the parameter index
						il.Emit(OpCodes.Ldelem_I4); // get the indirection index
						il.Emit(OpCodes.Callvirt, readOnlyCollectionGetItem); // get the item
					}
					il.Emit(OpCodes.Newobj, constructor);
					il.Emit(OpCodes.Ret);
					dynamicMethods.Add(constructor, result);
				}
				return result;
			}
		}

		public static Activator<T> CreateActivator<T>(SemanticNonterminalTypeFactory<T> target, ConstructorInfo constructor, int[] parameterMapping) where T: SemanticToken {
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			if (constructor == null) {
				throw new ArgumentNullException("constructor");
			}
			return (Activator<T>)GetDynamicMethod(constructor).CreateDelegate(typeof(Activator<T>), parameterMapping);
		}
	}
}
