using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Parser {
	[TestFixture]
	public class LalrProcessorTest: AssertionHelper {
		private int CountTokens(IToken currentToken) {
			int result = 1;
			Reduction reduction = currentToken as Reduction;
			if (reduction != null) {
				foreach (Token childToken in reduction.Children) {
					result += CountTokens(childToken);
				}
			}
			return result;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutTokenizer() {
			new LalrProcessor(null);
		}

		[Test]
		public void ParseTreeWithTrim() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer, true);
				Expect(processor.Trim, EqualTo(true));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.CommentLineRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.CommentBlockRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken.Symbol.Name, EqualTo("Expression"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(27));
			}
		}

		[Test]
		public void ParseTreeWithoutTrim() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer);
				Expect(processor.Trim, EqualTo(false));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.CommentLineRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.CommentBlockRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.TokenRead));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(TextToken)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken.Symbol.Name, EqualTo("Expression"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(39));
			}
		}
	}
}