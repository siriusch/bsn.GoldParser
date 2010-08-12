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

namespace bsn.GoldParser.Semantic {
	public class TestExpression<T>: TestValue where T: TestOperation {
		private readonly TestValue left;
		private readonly TestOperation operation;
		private readonly TestValue right;

		[Rule("<Expression> ::= <Expression> '+' <Mult Exp>", typeof(TestAdd))]
		[Rule("<Expression> ::= <Expression> '-' <Mult Exp>", typeof(TestSubtract))]
		[Rule("<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>", typeof(TestMultiply))]
		[Rule("<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>", typeof(TestDivide))]
		public TestExpression(TestValue left, T operation, TestValue right) {
			this.left = left;
			this.operation = operation;
			this.right = right;
		}

		public override double Compute() {
			return operation.Compute(left.Compute(), right.Compute());
		}
	}
}