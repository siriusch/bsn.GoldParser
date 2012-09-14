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
using System.Collections.ObjectModel;

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SymbolTypeMapTest {
		private readonly CompiledGrammar grammar;

		public SymbolTypeMapTest() {
			grammar = CgtCompiledGrammarTest.LoadCgtTestGrammar();
		}

		[Fact]
		public void CheckParentCommon() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.Equal(typeof(TestOperation), symbolTypeMap.GetSymbolType(symbol));
		}

		[Fact]
		public void CheckParentNone() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			Assert.Equal(typeof(TestToken), symbolTypeMap.GetSymbolType(symbol));
		}

		[Fact]
		public void CheckParentPassthrough() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.Equal(typeof(TestAdd), symbolTypeMap.GetSymbolType(symbol));
		}

		[Fact]
		public void CheckParentSkip() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.Equal(typeof(TestAdd), symbolTypeMap.GetSymbolType(symbol));
		}

		[Fact]
		public void CheckParentVersionIncrementType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			long version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.True(version < symbolTypeMap.Version);
			version = symbolTypeMap.Version;
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.True(version < symbolTypeMap.Version);
		}

		[Fact]
		public void CheckVersionIncrementType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			long version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Assert.True(version < symbolTypeMap.Version);
			version = symbolTypeMap.Version;
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			Assert.True(version < symbolTypeMap.Version);
		}

		[Fact]
		public void Create() {
			new SymbolTypeMap<SemanticToken>();
		}

		[Fact]
		public void GetCommonBaseTypeAncestor() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Assert.Equal(typeof(TestOperation), symbolTypeMap.GetCommonBaseType(typeof(TestAdd), typeof(TestSubtract)));
		}

		[Fact]
		public void GetCommonBaseTypeBothBase() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Assert.Equal(typeof(TestToken), symbolTypeMap.GetCommonBaseType(typeof(TestToken), typeof(TestToken)));
		}

		[Fact]
		public void GetCommonBaseTypeNoCommon() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Assert.Equal(typeof(TestToken), symbolTypeMap.GetCommonBaseType(typeof(TestValue), typeof(TestSubtract)));
		}

		[Fact]
		public void GetCommonBaseTypeSame() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Assert.Equal(typeof(TestValue), symbolTypeMap.GetCommonBaseType(typeof(TestValue), typeof(TestValue)));
		}

		[Fact]
		public void GetNoAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestToken));
			Assert.Equal(new[] {typeof(TestToken)}, baseTypes);
		}

		[Fact]
		public void GetSeveralAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestAdd));
			Assert.Equal(new[] {typeof(TestToken), typeof(TestOperation), typeof(TestAdd)}, baseTypes);
		}

		[Fact]
		public void GetSingleAncestor() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestOperation));
			Assert.Equal(new[] {typeof(TestToken), typeof(TestOperation)}, baseTypes);
		}

		[Fact]
		public void GetSymbolType() {
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			Assert.Equal(typeof(TestOperation), symbolTypeMap.GetSymbolType(symbol));
		}

		[Fact]
		public void InvalidType() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Assert.Throws<ArgumentException>(() => {
				symbolTypeMap.GetBaseTypes(typeof(SemanticToken));
			});
		}
	}
}
