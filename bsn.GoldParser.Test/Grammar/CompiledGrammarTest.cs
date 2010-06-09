using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace bsn.GoldParser.Grammar {
	[TestFixture]
	public class CompiledGrammarTest: AssertionHelper {
		internal static CompiledGrammar LoadTestGrammar() {
			return CompiledGrammar.Load(typeof(CompiledGrammarTest), "TestGrammar.cgt");
		}

		[Test]
		public void CheckDfaCharsetCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.DfaCharsetCount, EqualTo(11));
		}

		[Test]
		public void CheckAuthor() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.Author, EqualTo("Robert van Loenhout and Arsène von Wyss"));
		}

		[Test]
		public void CheckCaseSensitive() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.CaseSensitive, EqualTo(false));
		}

		[Test]
		public void CheckVersion() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.Version, EqualTo("1.1"));
		}

		[Test]
		public void CheckAbout() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.About, EqualTo("Example for testing Gold Parser Engine"));
		}

		[Test]
		public void CheckName() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.Name, EqualTo("Text Calculator Grammar"));
		}

		[Test]
		public void CheckEndSymbol() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.EndSymbol.Kind == SymbolKind.End);
			Expect(grammar.DfaInitialState.Index, EqualTo(0));
		}

		[Test]
		public void CheckErrorSymbol() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.ErrorSymbol.Kind == SymbolKind.Error);
			Expect(grammar.DfaInitialState.Index, EqualTo(0));
		}

		[Test]
		public void CheckInitialDfaStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.DfaInitialState, Not.Append(Null));
			Expect(grammar.DfaInitialState.Index, EqualTo(0));
		}

		[Test]
		public void CheckInitialLalrStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.InitialLRState, Not.Append(Null));
			Expect(grammar.InitialLRState.Index, EqualTo(0));
		}

		[Test]
		public void CheckLalrStateCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.LalrStateCount, EqualTo(20));
		}

		[Test]
		public void CheckRuleCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.RuleCount, EqualTo(11));
			for (int i = 0; i < grammar.RuleCount; i++) {
				Trace.WriteLine(grammar.GetRule(i).Definition, i.ToString());
			}
		}

		[Test]
		public void CheckSymbolCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.SymbolCount, EqualTo(18));
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Trace.WriteLine(grammar.GetSymbol(i).Name, i.ToString());
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutBinaryReader() {
			CompiledGrammar.Load((BinaryReader)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutManifestResourceName() {
			CompiledGrammar.Load(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutStream() {
			CompiledGrammar.Load((Stream)null);
		}

		[Test]
		public void GetTerminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("Float");
			Expect(symbol, Not.Null);
			Expect(symbol.Name, EqualTo("Float"));
		}

		[Test]
		public void GetNonterminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("<Expression>");
			Expect(symbol, Not.Null);
			Expect(symbol.Name, EqualTo("Expression"));
		}

		[Test]
		public void GetNonterminalRules() {
			CompiledGrammar grammar = LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName("<Expression>");
			Expect(symbol, Not.Null);
			ReadOnlyCollection<Rule> rules = grammar.GetRulesForSymbol(symbol);
			Expect(rules, Not.Null);
			Expect(rules.Count, EqualTo(3));
		}
	}
}