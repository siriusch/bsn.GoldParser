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

namespace bsn.GoldParser.Grammar {
	public class GrammarObjectSetTest {
		private readonly CompiledGrammar grammar;

		public GrammarObjectSetTest() {
			grammar = CgtCompiledGrammarTest.LoadCgtTestGrammar();
		}

		[Fact]
		public void Create() {
			new GrammarObjectSet<Symbol>();
		}

		[Fact]
		public void GetFalse() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Assert.Equal(false, set[grammar.GetSymbol(0)]);
		}

		[Fact]
		public void MultiSetGet() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = false;
			Assert.Equal(false, set[symbol]);
			set[symbol] = true;
			Assert.Equal(true, set[symbol]);
			set[symbol] = true;
			Assert.Equal(true, set[symbol]);
			set[symbol] = false;
			Assert.Equal(false, set[symbol]);
		}

		[Fact]
		public void SetGetFalse() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = false;
			Assert.Equal(false, set[symbol]);
		}

		[Fact]
		public void SetGetMulti() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			set[symbolX] = true;
			set[symbolY] = true;
			Assert.Equal(true, set[symbolX]);
			Assert.Equal(true, set[symbolY]);
			set[symbolX] = false;
			Assert.Equal(false, set[symbolX]);
			Assert.Equal(true, set[symbolY]);
		}

		[Fact]
		public void SetGetTrue() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = true;
			Assert.Equal(true, set[symbol]);
		}

		[Fact]
		public void SetNull() {
			GrammarObjectSet<Symbol> set = new GrammarObjectSet<Symbol>();
			Assert.Throws<ArgumentNullException>(() => {
				set[null] = true;
			});
		}
	}
}
