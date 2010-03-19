using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace bsn.GoldParser.Grammar {
	[TestFixture]
	public class CompiledGrammarTest: AssertionHelper {
		private static CompiledGrammar LoadTestGrammar() {
			return CompiledGrammar.Load(typeof(CompiledGrammarTest), "TestGrammar.cgt");
		}
	
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutBinaryReader() {
			CompiledGrammar.Load((BinaryReader)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutStream() {
			CompiledGrammar.Load((Stream)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutManifestResourceName() {
			CompiledGrammar.Load(null, null);
		}

		[Test]
		public void CheckSymbolCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.SymbolCount, EqualTo(15));
		}

		[Test]
		public void CheckRuleCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.RuleCount, EqualTo(11));
		}

		[Test]
		public void CheckDfaCharsetCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.DfaCharsetCount, EqualTo(11));
		}

		[Test]
		public void CheckLalrStateCount() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.LalrStateCount, EqualTo(20));
		}

		[Test]
		public void CheckInitialLalrStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.InitialLRState, Not.Append(Null));
			Expect(grammar.InitialLRState.Index, EqualTo(0));
		}

		[Test]
		public void CheckInitialDfaStateIndex() {
			CompiledGrammar grammar = LoadTestGrammar();
			Expect(grammar.DfaInitialState, Not.Append(Null));
			Expect(grammar.DfaInitialState.Index, EqualTo(0));
		}
	}
}
