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
					Type ownerType = typeof(SemanticNonterminalFactory<>).MakeGenericType(constructor.DeclaringType);
					result = new DynamicMethod(string.Format("SemanticNonterminalTypeFactory<{0}>.Activator", constructor.DeclaringType.FullName), constructor.DeclaringType, new[] {ownerType, typeof(ReadOnlyCollection<SemanticToken>)}, ownerType, true);
					ILGenerator il = result.GetILGenerator();
					int parameterCount = constructor.GetParameters().Length;
					for (int i = 0; i < parameterCount; i++) {
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldc_I4, i);
						il.Emit(OpCodes.Callvirt, readOnlyCollectionGetItem);
					}
					il.Emit(OpCodes.Newobj, constructor);
					il.Emit(OpCodes.Ret);
					dynamicMethods.Add(constructor, result);
				}
				return result;
			}
		}

		public static Activator<T> CreateActivator<T>(SemanticNonterminalTypeFactory<T> target, ConstructorInfo constructor) where T: SemanticToken {
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			if (constructor == null) {
				throw new ArgumentNullException("constructor");
			}
			return (Activator<T>)GetDynamicMethod(constructor).CreateDelegate(typeof(Activator<T>), target);
		}
	}
}
