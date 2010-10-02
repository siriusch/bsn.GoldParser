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
		public void CheckAbout() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.About, EqualTo("Example for testing Gold Parser Engine"));
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
		public void CheckDfaCharsetCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.DfaCharsetCount, EqualTo(16));
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
			Expect(grammar.LalrStateCount, EqualTo(24));
		}

		[Test]
		public void CheckName() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.Name, EqualTo("Text Calculator Grammar"));
		}

		[Test]
		public void CheckRuleCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.RuleCount, EqualTo(16));
			for (int i = 0; i < grammar.RuleCount; i++) {
				Trace.WriteLine(grammar.GetRule(i).Definition, i.ToString());
			}
		}

		[Test]
		public void CheckSymbolCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.SymbolCount, EqualTo(22));
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Trace.WriteLine(grammar.GetSymbol(i).Name, i.ToString());
			}
		}

		[Test]
		public void CheckVersion() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.Version, EqualTo("1.1"));
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
		public void GetNonterminalRules() {
			CompiledGrammar grammar = LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName("<Expression>");
			Expect(symbol, Not.Null);
			ReadOnlyCollection<Rule> rules = grammar.GetRulesForSymbol(symbol);
			Expect(rules, Not.Null);
			Expect(rules.Count, EqualTo(3));
		}

		[Test]
		public void GetNonterminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("<Expression>");
			Expect(symbol, Not.Null);
			Expect(symbol.Name, EqualTo("Expression"));
		}

		[Test]
		public void GetTerminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("Float");
			Expect(symbol, Not.Null);
			Expect(symbol.Name, EqualTo("Float"));
		}

		[Test]
		public void PackTest() {
			CompiledGrammar unpackedGrammar;
			CompiledGrammar packedGrammar;
			using (MemoryStream packedStream = new MemoryStream()) {
				using (Stream unpackedStream = typeof(CompiledGrammarTest).Assembly.GetManifestResourceStream(typeof(CompiledGrammarTest), "TestGrammar.cgt")) {
					Expect(unpackedStream, Not.Null);
					unpackedGrammar = CompiledGrammar.Load(unpackedStream);
					unpackedStream.Seek(0, SeekOrigin.Begin);
					CompiledGrammar.Pack(unpackedStream, packedStream, true);
					Debug.WriteLine(string.Format("Packed length: {0} (original {1}, {2} times reduction)", packedStream.Length, unpackedStream.Length, (double)unpackedStream.Length/(double)packedStream.Length));
					Expect(packedStream.Length < unpackedStream.Length);
				}
				packedStream.Seek(0, SeekOrigin.Begin);
				packedGrammar = CompiledGrammar.Load(packedStream);
			}
			Expect(packedGrammar.About, EqualTo(unpackedGrammar.About));
			Expect(packedGrammar.Author, EqualTo(unpackedGrammar.Author));
			Expect(packedGrammar.CaseSensitive, EqualTo(unpackedGrammar.CaseSensitive));
			Expect(packedGrammar.DfaCharsetCount, EqualTo(unpackedGrammar.DfaCharsetCount));
			Expect(packedGrammar.DfaInitialState.Index, EqualTo(unpackedGrammar.DfaInitialState.Index));
			Expect(packedGrammar.DfaStateCount, EqualTo(unpackedGrammar.DfaStateCount));
			Expect(packedGrammar.EndSymbol.Index, EqualTo(unpackedGrammar.EndSymbol.Index));
			Expect(packedGrammar.ErrorSymbol.Index, EqualTo(unpackedGrammar.ErrorSymbol.Index));
			Expect(packedGrammar.InitialLRState.Index, EqualTo(unpackedGrammar.InitialLRState.Index));
			Expect(packedGrammar.LalrStateCount, EqualTo(unpackedGrammar.LalrStateCount));
			Expect(packedGrammar.Name, EqualTo(unpackedGrammar.Name));
			Expect(packedGrammar.RuleCount, EqualTo(unpackedGrammar.RuleCount));
			Expect(packedGrammar.StartSymbol.Index, EqualTo(unpackedGrammar.StartSymbol.Index));
			Expect(packedGrammar.SymbolCount, EqualTo(unpackedGrammar.SymbolCount));
			Expect(packedGrammar.Version, EqualTo(unpackedGrammar.Version));
			for (int i = 0; i < packedGrammar.DfaCharsetCount; i++) {
				Expect(new string(packedGrammar.GetDfaCharset(i).CharactersIncludingSequence), EqualTo(new string(unpackedGrammar.GetDfaCharset(i).CharactersIncludingSequence)));
			}
		}
	}
}