using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.GoldParser.Semantic {
	internal static class SemanticTerminalTypeFactoryHelper {
		public delegate T Activator<T>(string text);

		private static readonly Dictionary<ConstructorInfo, DynamicMethod> dynamicMethods = new Dictionary<ConstructorInfo, DynamicMethod>();

		private static DynamicMethod GetDynamicMethod(ConstructorInfo constructor) {
			Debug.Assert(constructor != null);
			lock (dynamicMethods) {
				DynamicMethod result;
				if (!dynamicMethods.TryGetValue(constructor, out result)) {
					Type ownerType = typeof(SemanticTerminalFactory<>).MakeGenericType(constructor.DeclaringType);
					result = new DynamicMethod(string.Format("SemanticTerminalTypeFactory<{0}>.Activator", constructor.DeclaringType.FullName), constructor.DeclaringType, new[] {ownerType, typeof(string)}, ownerType, true);
					ILGenerator il = result.GetILGenerator();
					foreach (ParameterInfo parameterInfo in constructor.GetParameters()) {
						if ((parameterInfo.Position > 0) || (parameterInfo.ParameterType != typeof(string))) {
							throw new ArgumentException("The constructor may have at most exactly one string parameter");
						}
						il.Emit(OpCodes.Ldarg_1);
					}
					il.Emit(OpCodes.Newobj, constructor);
					il.Emit(OpCodes.Ret);
					dynamicMethods.Add(constructor, result);
				}
				return result;
			}
		}

		public static Activator<T> CreateActivator<T>(SemanticTerminalTypeFactory<T> target, ConstructorInfo constructor) where T: SemanticToken {
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
