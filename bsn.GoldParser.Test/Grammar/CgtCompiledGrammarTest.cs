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
using System.Diagnostics;
using System.IO;

using Xunit;

namespace bsn.GoldParser.Grammar {
	public class CgtCompiledGrammarTest: CompiledGrammarTestBase {
		protected internal static CompiledGrammar LoadCgtTestGrammar() {
			return CompiledGrammar.Load(typeof(CgtCompiledGrammarTest), "TestGrammar.cgt");
		}

		[Fact]
		public void CheckCaseSensitive() {
			CompiledGrammar grammar = LoadTestGrammar();
#pragma warning disable 612,618
			Assert.Equal(false, grammar.CaseSensitive);
#pragma warning restore 612,618
		}

		[Fact]
		public void CheckDfaCharsetCount() {
			CheckDfaCharsetCountInternal(16);
		}

		[Fact]
		public void CheckSymbolCount() {
			CheckSymcolCountInternal(22);
		}

		[Fact]
		public void PackTest() {
			CompiledGrammar unpackedGrammar;
			CompiledGrammar packedGrammar;
			using (MemoryStream packedStream = new MemoryStream()) {
				using (Stream unpackedStream = typeof(CgtCompiledGrammarTest).Assembly.GetManifestResourceStream(typeof(CgtCompiledGrammarTest), "TestGrammar.cgt")) {
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

		protected override CompiledGrammar LoadTestGrammar() {
			return LoadCgtTestGrammar();
		}
	}
}
