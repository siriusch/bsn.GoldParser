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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.GoldParser.Semantic {
	internal static class SemanticTerminalTypeFactoryHelper<TBase> where TBase: SemanticToken {
		public delegate T Activator<out T>(string text) where T: TBase;

		private struct MethodKey: IEquatable<MethodKey> {
			private readonly int additionalArguments;
			private readonly MethodBase method;

			public MethodKey(MethodBase method, int additionalArguments) {
				this.method = method;
				this.additionalArguments = additionalArguments;
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				return obj is MethodKey && Equals((MethodKey)obj);
			}

			public override int GetHashCode() {
				unchecked {
					return (method.GetHashCode()*397)^additionalArguments.GetHashCode();
				}
			}

			public bool Equals(MethodKey other) {
				return method.Equals(other.method) && additionalArguments.Equals(other.additionalArguments);
			}
		}

		private static readonly Dictionary<MethodKey, DynamicMethod> dynamicMethods = new Dictionary<MethodKey, DynamicMethod>();

		public static Activator<T> CreateActivator<T>(SemanticTerminalTypeFactory<TBase, T> target, MethodBase method, object[] additionalArguments) where T: TBase {
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			return (Activator<T>)GetDynamicMethod(method, additionalArguments.Length).CreateDelegate(typeof(Activator<T>), additionalArguments);
		}

		private static DynamicMethod GetDynamicMethod(MethodBase methodBase, int additionalArguments) {
			Debug.Assert(methodBase != null);
			lock (dynamicMethods) {
				DynamicMethod result;
				MethodKey key = new MethodKey(methodBase, additionalArguments);
				if (!dynamicMethods.TryGetValue(key, out result)) {
					ConstructorInfo constructor;
					MethodInfo method;
					Type returnType = SemanticTypeFactoryHelper.GetReturnTypeOfMethodBase<TBase>(methodBase, out constructor, out method);
					result = new DynamicMethod(string.Format("SemanticTerminalTypeFactory<{0}>.Activator", returnType.FullName), returnType, new[] {typeof(object[]), typeof(string)}, true);
					ILGenerator il = result.GetILGenerator();
					ParameterInfo[] parameters = methodBase.GetParameters();
					if (parameters.Length > 0) {
						Array.Sort(parameters, (x, y) => x.Position-y.Position);
						int parameter = 0;
						if ((parameters[parameter].ParameterType == typeof(string)) && (parameters.Length > additionalArguments)) {
							il.Emit(OpCodes.Ldarg_1);
							parameter = 1;
						}
						int additionalArgument = 0;
						while (parameter < parameters.Length) {
							Type parameterType = parameters[parameter++].ParameterType;
							if (additionalArgument < additionalArguments) {
								il.Emit(OpCodes.Ldarg_0); // load the object[]
								il.Emit(OpCodes.Ldc_I4, additionalArgument++); // load the parameter index
								il.Emit(OpCodes.Ldelem_Ref); // get the additional parameter
							} else {
								if ((!parameterType.IsValueType) || (Nullable.GetUnderlyingType(parameterType) != null)) {
									il.Emit(OpCodes.Ldnull);
								} else {
									throw new InvalidOperationException("An additional, non-nullable argument is required");
								}
							}
							il.Emit(OpCodes.Unbox_Any, parameterType); // make the verifier happy by casting/unboxing the reference
						}
					}
					if (constructor != null) {
						il.Emit(OpCodes.Newobj, constructor); // invoke constructor
					} else {
						il.Emit(OpCodes.Call, method); // invoke static method
					}
					il.Emit(OpCodes.Ret);
					dynamicMethods.Add(key, result);
				}
				return result;
			}
		}
	}
}
