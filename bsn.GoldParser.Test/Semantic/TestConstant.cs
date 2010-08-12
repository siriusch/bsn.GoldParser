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
using System.Globalization;

namespace bsn.GoldParser.Semantic {
	[Terminal("Integer", typeof(int))]
	[Terminal("Float", typeof(double))]
	public class TestConstant<T>: TestValue where T: struct, IConvertible {
		private readonly T constant;

		public TestConstant(string constant) {
			this.constant = (T)Convert.ChangeType(constant, typeof(T), NumberFormatInfo.InvariantInfo);
		}

		public override double Compute() {
			return constant.ToDouble(NumberFormatInfo.InvariantInfo);
		}
	}
}