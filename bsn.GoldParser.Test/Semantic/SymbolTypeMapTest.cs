// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Ars�ne von Wyss - avw@gmx.ch
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
using System.Collections.ObjectModel;
using System.Linq;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SymbolTypeMapTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void CheckParentCommon() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void CheckParentNone() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void CheckParentPassthrough() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestAdd)));
		}

		[Test]
		public void CheckParentSkip() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestAdd)));
		}

		[Test]
		public void CheckParentVersionIncrementType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			long version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.Version, GreaterThan(version));
			version = symbolTypeMap.Version;
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.Version, GreaterThan(version));
		}

		[Test]
		public void CheckVersionIncrementType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			long version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.Version, GreaterThan(version));
			version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			Expect(symbolTypeMap.Version, GreaterThan(version));
		}

		[Test]
		public void Create() {
			new SymbolTypeMap<SemanticToken>();
		}

		[Test]
		public void GetCommonBaseTypeAncestor() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestAdd), typeof(TestSubtract)), EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void GetCommonBaseTypeBothBase() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestToken), typeof(TestToken)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetCommonBaseTypeNoCommon() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestValue), typeof(TestSubtract)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetCommonBaseTypeSame() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestValue), typeof(TestValue)), EqualTo(typeof(TestValue)));
		}

		[Test]
		public void GetNoAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestToken));
			Expect(baseTypes, EquivalentTo(new[] {typeof(TestToken)}));
		}

		[Test]
		public void GetSeveralAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestAdd));
			Expect(baseTypes, EquivalentTo(new[] {typeof(TestToken), typeof(TestOperation), typeof(TestAdd)}));
		}

		[Test]
		public void GetSingleAncestor() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestOperation));
			Expect(baseTypes, EquivalentTo(new[] {typeof(TestToken), typeof(TestOperation)}));
		}

		[Test]
		public void GetSymbolType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestOperation)));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidType() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			symbolTypeMap.GetBaseTypes(typeof(SemanticToken));
		}
	}
}