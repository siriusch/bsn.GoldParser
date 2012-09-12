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

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class RuleTrimAttributeTest {
		private readonly CompiledGrammar grammar;

		public RuleTrimAttributeTest() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Fact]
		public void BindToGrammar() {
			RuleTrimAttribute attribute = new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 1);
			Assert.NotNull(attribute.Bind(grammar));
			Assert.Equal(1, attribute.TrimSymbolIndex);
		}

		[Fact]
		public void ConstructWithEmptyString() {
			Assert.Throws<ArgumentNullException>(() => {
				new RuleTrimAttribute(string.Empty, 0);
			});
		}

		[Fact]
		public void ConstructWithInvalidTrimName() {
			Assert.Throws<ArgumentException>(() => {
				new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", "Value");
			});
		}

		[Fact]
		public void ConstructWithNegativeIndex() {
			Assert.Throws<ArgumentOutOfRangeException>(() => {
				new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", -1);
			});
		}

		[Fact]
		public void ConstructWithNullTrimName() {
			Assert.Throws<ArgumentNullException>(() => {
				new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", null);
			});
		}

		[Fact]
		public void ConstructWithOverflowIndex() {
			Assert.Throws<ArgumentOutOfRangeException>(() => {
				new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", 2);
			});
		}

		[Fact]
		public void ConstructWithTrimName() {
			new RuleTrimAttribute("<Negate Exp> ::= '-' <Value>", "<Value>");
		}

		[Fact]
		public void ConstructWithoutString() {
			Assert.Throws<ArgumentNullException>(() => {
				new RuleTrimAttribute(null, 0);
			});
		}
	}
}
