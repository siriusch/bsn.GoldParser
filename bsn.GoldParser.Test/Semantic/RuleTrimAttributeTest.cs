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

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleTrimAttributeTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void BindToGrammar() {
			RuleTrimAttribute attribute = new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 1);
			Expect(attribute.Bind(grammar), Not.Null);
			Expect(attribute.TrimSymbolIndex, EqualTo(1));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithEmptyString() {
			new RuleTrimAttribute(string.Empty, 0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ConstructWithInvalidTrimName() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", "Value");
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ConstructWithNegativeIndex() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", -1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithNullTrimName() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ConstructWithOverflowIndex() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 2);
		}

		[Test]
		public void ConstructWithTrimName() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", "<Value>");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutString() {
			new RuleTrimAttribute(null, 0);
		}
	}
}