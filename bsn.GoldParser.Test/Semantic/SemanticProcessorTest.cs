using System;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class SemanticProcessorTest: AssertionHelper {
		private SemanticTypeActions<TestToken> actions;

		[TestFixtureSetUp]
		public void SetUp() {
			actions = new SemanticTypeActions<TestToken>(CompiledGrammarTest.LoadTestGrammar());
		}

		[Test]
		public void ParseComplexExpression() {
			using (TestStringReader reader = new TestStringReader("((100+5.0)/(4.5+.5))-12345.4e+1")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Expect(processor.ParseAll(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken, InstanceOf<TestValue>());
				TestValue value = (TestValue)processor.CurrentToken;
				Expect(value.Compute(), EqualTo(-123433.0));
			}
		}

		[Test]
		public void ParseEmpty() {
			using (TestStringReader reader = new TestStringReader("")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Expect(processor.ParseAll(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken, InstanceOf<TestEmpty>());
			}
		}

		[Test]
		public void ParseNull() {
			using (TestStringReader reader = new TestStringReader("NULL")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Expect(processor.ParseAll(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken, InstanceOf<TestEmpty>());
			}
		}

		[Test]
		public void ParseSimpleExpression() {
			using (TestStringReader reader = new TestStringReader("100")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Expect(processor.ParseAll(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken, InstanceOf<TestValue>());
				TestValue value = (TestValue)processor.CurrentToken;
				Expect(value.Compute(), EqualTo(100));
			}
		}
	}
}
