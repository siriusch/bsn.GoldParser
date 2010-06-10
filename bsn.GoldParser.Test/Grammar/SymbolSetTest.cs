using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace bsn.GoldParser.Grammar {
	[TestFixture]
	public class SymbolSetTest: AssertionHelper {
		[Test]
		public void Create() {
			new SymbolSet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetNull() {
			SymbolSet set = new SymbolSet();
			set[null] = true;
		}

		[Test]
		public void GetFalse() {
			SymbolSet set = new SymbolSet();
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Expect(set[grammar.GetSymbol(0)], EqualTo(false));
		}

		[Test]
		public void SetGetFalse() {
			SymbolSet set = new SymbolSet();
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = false;
			Expect(set[symbol], EqualTo(false));
		}

		[Test]
		public void SetGetTrue() {
			SymbolSet set = new SymbolSet();
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbol(0);
			set[symbol] = true;
			Expect(set[symbol], EqualTo(true));
		}

		[Test]
		public void SetGetMulti() {
			SymbolSet set = new SymbolSet();
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
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
		public void MultiSetGet() {
			SymbolSet set = new SymbolSet();
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
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
	}
}
