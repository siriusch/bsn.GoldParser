using System;
using System.Collections.ObjectModel;
using System.Linq;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SymbolTypeMapTest: AssertionHelper {
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
		public void GetCommonBaseTypeNoCommon() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestValue), typeof(TestSubtract)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetCommonBaseTypeSame() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			Expect(symbolTypeMap.GetCommonBaseType(typeof(TestToken), typeof(TestToken)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetEmptyAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestToken));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(0));
		}

		[Test]
		public void GetSeveralAncestors() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestAdd));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(2));
			Expect(baseTypes[0], EqualTo(typeof(TestToken)));
			Expect(baseTypes[1], EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void GetSingleAncestor() {
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			ReadOnlyCollection<Type> baseTypes = symbolTypeMap.GetBaseTypes(typeof(TestOperation));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(1));
			Expect(baseTypes[0], EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetSymbolType() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>();
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void CheckVersionIncrementType() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
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
		public void CheckParentVersionIncrementType() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
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
		public void CheckParentPassthrough() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestAdd)));
		}

		[Test]
		public void CheckParentSkip() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestAdd)));
		}

		[Test]
		public void CheckParentNone() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			Expect(symbolTypeMap.GetSymbolType(symbol), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void CheckParentCommon() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName(Symbol.FormatTerminalSymbol("-"));
			SymbolTypeMap<TestToken> parentSymbolTypeMap = new SymbolTypeMap<TestToken>();
			SymbolTypeMap<TestToken> symbolTypeMap = new SymbolTypeMap<TestToken>(parentSymbolTypeMap);
			parentSymbolTypeMap.SetTypeForSymbol(symbol, typeof(TestSubtract));
			symbolTypeMap.SetTypeForSymbol(symbol, typeof(TestAdd));
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
