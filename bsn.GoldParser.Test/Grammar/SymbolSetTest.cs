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
using System.Linq;

using NUnit.Framework;

namespace bsn.GoldParser.Grammar {
	[TestFixture]
	public class SymbolSetTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void Create() {
			new SymbolSet();
		}

		[Test]
		public void GetFalse() {
			SymbolSet set = new SymbolSet();
			Expect(set[grammar.GetSymbol(0)], EqualTo(false));
		}

		[Test]
		public void MultiSetGet() {
			SymbolSet set = new SymbolSet();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = false;
			Expect(set[symbol], EqualTo(false));
			set[symbol] = true;
			Expect(set[symbol], EqualTo(true));
			set[symbol] = true;
			Expect(set[symbol], EqualTo(true));
			set[symbol] = false;
			Expect(set[symbol], EqualTo(false));
		}

		[Test]
		public void SetGetFalse() {
			SymbolSet set = new SymbolSet();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = false;
			Expect(set[symbol], EqualTo(false));
		}

		[Test]
		public void SetGetMulti() {
			SymbolSet set = new SymbolSet();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			set[symbolX] = true;
			set[symbolY] = true;
			Expect(set[symbolX], EqualTo(true));
			Expect(set[symbolY], EqualTo(true));
			set[symbolX] = false;
			Expect(set[symbolX], EqualTo(false));
			Expect(set[symbolY], EqualTo(true));
		}

		[Test]
		public void SetGetTrue() {
			SymbolSet set = new SymbolSet();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = true;
			Expect(set[symbol], EqualTo(true));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetNull() {
			SymbolSet set = new SymbolSet();
			set[null] = true;
		}
	}
}