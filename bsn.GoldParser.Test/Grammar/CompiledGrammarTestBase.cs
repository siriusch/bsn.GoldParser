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

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using Xunit;

namespace bsn.GoldParser.Grammar {
	public abstract class CompiledGrammarTestBase {
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

		protected virtual void CheckDfaCharsetCountInternal(int expectedCount) {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(expectedCount, grammar.DfaCharsetCount);
		}

		protected virtual void CheckSymcolCountInternal(int expectedCount) {
			CompiledGrammar grammar = LoadTestGrammar();
			Assert.Equal(expectedCount, grammar.SymbolCount);
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Trace.WriteLine(grammar.GetSymbol(i).Name, i.ToString());
			}
		}

		protected abstract CompiledGrammar LoadTestGrammar();
	}
}
