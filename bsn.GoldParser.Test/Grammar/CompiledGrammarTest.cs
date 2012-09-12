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

using Xunit;

namespace bsn.GoldParser.Grammar {
	public class CompiledGrammarTest {
		internal static CompiledGrammar LoadTestGrammar() {
			return CompiledGrammar.Load(typeof(CompiledGrammarTest), "TestGrammar.cgt");
		}

		[Fact]
		public void CheckAbout() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal("Example for testing Gold Parser Engine", grammar.About);
		}

		[Fact]
		public void CheckAuthor() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal("Robert van Loenhout and Arsène von Wyss", grammar.Author);
		}

		[Fact]
		public void CheckCaseSensitive() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(false, grammar.CaseSensitive);
		}

		[Fact]
		public void CheckDfaCharsetCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(16, grammar.DfaCharsetCount);
		}

		[Fact]
		public void CheckEndSymbol() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(SymbolKind.End, grammar.EndSymbol.Kind);
			Assert.Equal(0, grammar.DfaInitialState.Index);
		}

		[Fact]
		public void CheckErrorSymbol() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(SymbolKind.Error, grammar.ErrorSymbol.Kind);
			Assert.Equal(0, grammar.DfaInitialState.Index);
		}

		[Fact]
		public void CheckInitialDfaStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.NotNull(grammar.DfaInitialState);
			Assert.Equal(0, grammar.DfaInitialState.Index);
		}

		[Fact]
		public void CheckInitialLalrStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.NotNull(grammar.InitialLRState);
			Assert.Equal(0, grammar.InitialLRState.Index);
		}

		[Fact]
		public void CheckLalrStateCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(24, grammar.LalrStateCount);
		}

		[Fact]
		public void CheckName() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal("Text Calculator Grammar", grammar.Name);
		}

		[Fact]
		public void CheckRuleCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(16, grammar.RuleCount);
			for (int i = 0; i < grammar.RuleCount; i++) {
				Trace.WriteLine(grammar.GetRule(i).Definition, i.ToString());
			}
		}

		[Fact]
		public void CheckSymbolCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(22, grammar.SymbolCount);
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Trace.WriteLine(grammar.GetSymbol(i).Name, i.ToString());
			}
		}

		[Fact]
		public void CheckVersion() {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal("1.1", grammar.Version);
		}

		[Fact]
		public void ConstructWithoutBinaryReader() {
			Assert.Throws<ArgumentNullException>(() => {
				CompiledGrammar.Load((BinaryReader)null);
			});
		}

		[Fact]
		public void ConstructWithoutManifestResourceName() {
			Assert.Throws<ArgumentNullException>(() => {
				CompiledGrammar.Load(null, null);
			});
		}

		[Fact]
		public void ConstructWithoutStream() {
			Assert.Throws<ArgumentNullException>(() => {
				CompiledGrammar.Load((Stream)null);
			});
		}

		[Fact]
		public void GetNonterminalRules() {
			CompiledGrammar grammar = LoadTestGrammar();
			Symbol symbol = grammar.GetSymbolByName("<Expression>");
			Assert.NotNull(symbol);
			ReadOnlyCollection<Rule> rules = grammar.GetRulesForSymbol(symbol);
			Assert.NotNull(rules);
			Assert.Equal(3, rules.Count);
		}

		[Fact]
		public void GetNonterminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("<Expression>");
			Assert.NotNull(symbol);
			Assert.Equal("Expression", symbol.Name);
		}

		[Fact]
		public void GetTerminalSymbol() {
			Symbol symbol = LoadTestGrammar().GetSymbolByName("Float");
			Assert.NotNull(symbol);
			Assert.Equal("Float", symbol.Name);
		}

		[Fact]
		public void PackTest() {
			CompiledGrammar unpackedGrammar;
			CompiledGrammar packedGrammar;
			using (MemoryStream packedStream = new MemoryStream()) {
				using (Stream unpackedStream = typeof(CompiledGrammarTest).Assembly.GetManifestResourceStream(typeof(CompiledGrammarTest), "TestGrammar.cgt")) {
					Assert.NotNull(unpackedStream);
					unpackedGrammar = CompiledGrammar.Load(unpackedStream);
					unpackedStream.Seek(0, SeekOrigin.Begin);
					CompiledGrammar.Pack(unpackedStream, packedStream, true);
					Debug.WriteLine(string.Format("Packed length: {0} (original {1}, {2} times reduction)", packedStream.Length, unpackedStream.Length, unpackedStream.Length/(double)packedStream.Length));
					Assert.True(packedStream.Length < unpackedStream.Length);
				}
				packedStream.Seek(0, SeekOrigin.Begin);
				packedGrammar = CompiledGrammar.Load(packedStream);
			}
			Assert.Equal(unpackedGrammar.About, packedGrammar.About);
			Assert.Equal(unpackedGrammar.Author, packedGrammar.Author);
			Assert.Equal(unpackedGrammar.CaseSensitive, packedGrammar.CaseSensitive);
			Assert.Equal(unpackedGrammar.DfaCharsetCount, packedGrammar.DfaCharsetCount);
			Assert.Equal(unpackedGrammar.DfaInitialState.Index, packedGrammar.DfaInitialState.Index);
			Assert.Equal(unpackedGrammar.DfaStateCount, packedGrammar.DfaStateCount);
			Assert.Equal(unpackedGrammar.EndSymbol.Index, packedGrammar.EndSymbol.Index);
			Assert.Equal(unpackedGrammar.ErrorSymbol.Index, packedGrammar.ErrorSymbol.Index);
			Assert.Equal(unpackedGrammar.InitialLRState.Index, packedGrammar.InitialLRState.Index);
			Assert.Equal(unpackedGrammar.LalrStateCount, packedGrammar.LalrStateCount);
			Assert.Equal(unpackedGrammar.Name, packedGrammar.Name);
			Assert.Equal(unpackedGrammar.RuleCount, packedGrammar.RuleCount);
			Assert.Equal(unpackedGrammar.StartSymbol.Index, packedGrammar.StartSymbol.Index);
			Assert.Equal(unpackedGrammar.SymbolCount, packedGrammar.SymbolCount);
			Assert.Equal(unpackedGrammar.Version, packedGrammar.Version);
			for (int i = 0; i < packedGrammar.DfaCharsetCount; i++) {
				Assert.Equal(new string(unpackedGrammar.GetDfaCharset(i).CharactersIncludingSequence), new string(packedGrammar.GetDfaCharset(i).CharactersIncludingSequence));
			}
		}
	}
}
