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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.GoldParser.Semantic {
	internal static class SemanticNonterminalTypeFactoryHelper<TBase> where TBase: SemanticToken {
		public delegate T Activator<T>(IList<TBase> tokens);

		private static readonly Dictionary<ConstructorInfo, DynamicMethod> dynamicMethods = new Dictionary<ConstructorInfo, DynamicMethod>();
		private static readonly MethodInfo iListGetItem = GetIListGetItemMethod();

		private static MethodInfo GetIListGetItemMethod() {
			MethodInfo result = typeof(IList<TBase>).GetProperty("Item").GetGetMethod();
			Debug.Assert(result != null);
			return result;
		}

		private static DynamicMethod GetDynamicMethod(ConstructorInfo constructor) {
			Debug.Assert(constructor != null);
			lock (dynamicMethods) {
				DynamicMethod result;
				if (!dynamicMethods.TryGetValue(constructor, out result)) {
					result = new DynamicMethod(string.Format("SemanticNonterminalTypeFactory<{0}>.Activator", constructor.DeclaringType.FullName), constructor.DeclaringType, new[] {typeof(int[]), typeof(IList<TBase>)}, true);
					ILGenerator il = result.GetILGenerator();
					Dictionary<int, ParameterInfo> parameters = new Dictionary<int, ParameterInfo>();
					foreach (ParameterInfo parameter in constructor.GetParameters()) {
						parameters.Add(parameter.Position, parameter);
					}
					for (int i = 0; i < parameters.Count; i++) {
						if (parameters[i].ParameterType.IsValueType) {
							throw new InvalidOperationException("Constructor arguments cannot be value types");
						}
						Label loadNull = il.DefineLabel();
						Label end = il.DefineLabel();
						il.Emit(OpCodes.Ldarg_1); // load the IList<TBase>
						il.Emit(OpCodes.Ldarg_0); // load the int[]
						il.Emit(OpCodes.Ldc_I4, i); // load the parameter index
						il.Emit(OpCodes.Ldelem_I4); // get the indirection index
						il.Emit(OpCodes.Dup); // copy the indicrection index
						il.Emit(OpCodes.Ldc_I4_0); // and load a 0
						il.Emit(OpCodes.Blt_S, loadNull); // compare the stored indicrection index and the stored 0, if less (e.g. -1) we need to load a null
						il.Emit(OpCodes.Callvirt, iListGetItem); // otherwise get the item
						il.Emit(OpCodes.Castclass, parameters[i].ParameterType); // make the verifier happy by casting the reference
						il.Emit(OpCodes.Br_S, end); // jump to end
						il.MarkLabel(loadNull);
						il.Emit(OpCodes.Pop); // pop the unused indirection index
						il.Emit(OpCodes.Pop); // pop the unused reference to the IList<TBase>
						il.Emit(OpCodes.Ldnull); // load a null reference instead
						il.MarkLabel(end);
					}
					il.Emit(OpCodes.Newobj, constructor); // invoke methodBase
					il.Emit(OpCodes.Ret);
					dynamicMethods.Add(constructor, result);
				}
				return result;
			}
		}

		public static Activator<T> CreateActivator<T>(SemanticNonterminalTypeFactory<TBase, T> target, MethodBase methodBase, int[] parameterMapping) where T: TBase {
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			if (methodBase == null) {
				throw new ArgumentNullException("methodBase");
			}
            if(methodBase is ConstructorInfo)
            {
                return (Activator<T>)GetDynamicMethod(methodBase as ConstructorInfo).CreateDelegate(typeof(Activator<T>), parameterMapping);
            }
            if (methodBase is MethodInfo)
            {
                return (tokens) => (T) methodBase.Invoke(null, Map(tokens, parameterMapping));
            }
            throw new ArgumentException("Expected methodBase to be one of: ConstructorInfo, MethodInfo, instead is: " + methodBase.GetType());			
		}

	    private static object[] Map(IList<TBase> tokens, int[] parameterMapping)
	    {
	        var res = new object[parameterMapping.Length];
            for (var i = 0; i < res.Length;i++)
            {
                if (parameterMapping[i] < 0)
                    res[i] = null;
                else
                    res[i] = tokens[parameterMapping[i]];
            }
	        return res;
	    }
	}
}