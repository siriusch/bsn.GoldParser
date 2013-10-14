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
using System.Diagnostics;
using System.Reflection;

namespace bsn.GoldParser.Semantic {
	internal static class SemanticTypeFactoryHelper {
		internal static Type GetReturnTypeOfMethodBase<TBase>(MethodBase methodBase, out ConstructorInfo constructor, out MethodInfo method) where TBase: SemanticToken {
			constructor = methodBase as ConstructorInfo;
			method = methodBase as MethodInfo;
			Type returnType;
			if (constructor != null) {
				returnType = constructor.DeclaringType;
				Debug.Assert(returnType != null);
			} else if (method != null) {
				if (!method.IsStatic) {
					throw new InvalidOperationException("Factories can only be created for static methods");
				}
				returnType = method.ReturnType;
				if (!typeof(TBase).IsAssignableFrom(returnType)) {
					throw new InvalidOperationException("The static method doesn't return the required type");
				}
			} else {
				throw new ArgumentException("Expected methodBase to be one of: ConstructorInfo, MethodInfo, instead is: "+methodBase.GetType());
			}
			return returnType;
		}
	}
}
