// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
//
// This file has kinly been contributed by Jan Polasek
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

namespace bsn.GoldParser.Semantic.StrictCtorParameterMatching {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	[Terminal("-")]
	[Terminal("(")]
	[Terminal(")")]
	[Terminal("*")]
	[Terminal("/")]
	[Terminal("+")]
	[Terminal("Float")]
	[Terminal("Integer")]
	[Terminal("NULL")]
	[Terminal("String")]
	internal class MockTokenBaseExplicitNoChecks: SemanticToken {
		public MockTokenBaseExplicitNoChecks(string text) {}

		[Rule(@"<Empty> ::= ", false)]
		[Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
		[Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
		[Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
		[Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
		[Rule(@"<Negate Exp> ::= '-' <Value>", false)]
		[Rule(@"<Value> ::= '(' <Expression> ')'")]
		public MockTokenBaseExplicitNoChecks(MockTokenBaseExplicitNoChecks a, MockTokenBaseExplicitNoChecks b, MockTokenBaseExplicitNoChecks c) {}
	}
}
