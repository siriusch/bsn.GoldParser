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
using System.Diagnostics;
using System.Reflection;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// The factory for terminals of the semantic token type implementation.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	/// <typeparam name="TOutput">The <see cref="SemanticToken"/> descendant instantiated by this factory.</typeparam>
	public class SemanticTerminalTypeFactory<TBase, TOutput>: SemanticTerminalFactory<TBase, TOutput> where TBase: SemanticToken where TOutput: TBase {
		private static bool IsCallCompatible(ParameterInfo[] parameters, int offset, Type[] types) {
			if ((parameters.Length-offset) != types.Length) {
				// mismatch of count
				return false;
			}
			for (int i = 0; i < types.Length; i++) {
				Type parameterType = parameters[i+offset].ParameterType;
				if (types[i] == null) {
					if (parameterType.IsValueType && (Nullable.GetUnderlyingType(parameterType) == null)) {
						return false;
					}
				} else {
					if (!parameterType.IsAssignableFrom(types[i])) {
						return false;
					}
				}
			}
			return true;
		}

		private static ConstructorInfo FindMatchingConstructor(object[] additionalArguments) {
			Type[] additionalTypes = Array.ConvertAll(additionalArguments, o => o == null ? null : o.GetType());
			ConstructorInfo[] constructors = typeof(TOutput).GetConstructors();
			foreach (ConstructorInfo constructor in constructors) {
				ParameterInfo[] parameters = constructor.GetParameters();
				if ((parameters.Length > 0) && (parameters[0].ParameterType == typeof(string)) && IsCallCompatible(parameters, 1, additionalTypes)) {
					return constructor;
				}
			}
			foreach (ConstructorInfo constructor in constructors) {
				ParameterInfo[] parameters = constructor.GetParameters();
				if (IsCallCompatible(parameters, 0, additionalTypes)) {
					return constructor;
				}
			}
			throw new InvalidOperationException("No matching constructor found");
		}

		private readonly SemanticTerminalTypeFactoryHelper<TBase>.Activator<TOutput> activator;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemanticTerminalTypeFactory&lt;TBase, TOutput&gt;"/> class. This is mainly for internal use.
		/// </summary>
		public SemanticTerminalTypeFactory(MethodBase methodBase, object[] additionalArguments) {
			if (additionalArguments == null) {
				additionalArguments = new object[0];
			}
			if (methodBase == null) {
				methodBase = FindMatchingConstructor(additionalArguments);
			}
			activator = SemanticTerminalTypeFactoryHelper<TBase>.CreateActivator(this, methodBase, additionalArguments);
			Debug.Assert(activator != null);
		}

		protected override TOutput Create(string text) {
			return activator(text);
		}
	}
}