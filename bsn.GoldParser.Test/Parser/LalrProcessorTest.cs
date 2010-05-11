using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Parser {
	[TestFixture]
	public class LalrProcessorTest: AssertionHelper {
		private int CountTokens(Token currentToken) {
			int result = 1;
			foreach (Token token in currentToken.Children) {
				result += CountTokens(token);
			}
			return result;
		}

		[Test]
		[ExpectedException]
		public void ConstructWithoutInitialState() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
				new LalrProcessor(tokenizer, null);
			}
		}

		[Test]
		[ExpectedException]
		public void ConstructWithoutTokenizer() {
			new LalrProcessor(null, CompiledGrammarTest.LoadTestGrammar().InitialLRState);
		}

		[Test]
		public void ParseTreeWithTrim() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
				LalrProcessor processor = new LalrProcessor(tokenizer, grammar.InitialLRState, true);
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
				Expect(processor.CurrentToken.ParentSymbol.Name, EqualTo("Expression"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(27));
			}
		}

		[Test]
		public void ParseTreeWithoutTrim() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
				LalrProcessor processor = new LalrProcessor(tokenizer, grammar.InitialLRState);
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
				Expect(processor.CurrentToken.ParentSymbol.Name, EqualTo("Expression"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(39));
			}
		}
	}
}