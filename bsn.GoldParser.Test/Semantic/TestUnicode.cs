using System;
using System.IO;
using System.Text;

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class TestUnicode {
		public sealed class TestParser {
			public static bool Parse(string filePath) {
				using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
					return Parse(stream);
				}
			}

			public static bool Parse(Stream stream) {
				CompiledGrammar grammar = CompiledGrammar.Load(typeof(TestUnicode), "TestUnicode.cgt"); // embedded resource
				SemanticTypeActions<TestToken> actions = new SemanticTypeActions<TestToken>(grammar);
				actions.Initialize(true);
				using (StreamReader reader = new StreamReader(stream)) // defaults to UTF-8
				{
					SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
					ParseMessage parseMessage = processor.ParseAll();
					return (parseMessage == ParseMessage.Accept);
				}
			}
		}

		[Terminal("(EOF)")]
		[Terminal("(Error)")]
		[Terminal("(Whitespace)")]
		[Terminal("Identifier")]
		[Terminal("från")]
		[Terminal("välj")]
		[Terminal("Åäö")]
		public class TestToken: SemanticToken {
			public TestToken() {}

			[Rule("<Select> ::= ~välj Åäö från <Källa>")]
			public TestToken(TestToken t1, TestToken t2, TestToken t3) {}
		}

		[Fact]
		public void TestSemanticMappingWithUnicodeStrings() {
			string source = "välj ä från dummy";
			Assert.True(TestParser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(source))));
		}
	}
}
