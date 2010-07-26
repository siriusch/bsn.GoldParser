using System;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Parser {
	[TestFixture]
	public class TokenizerTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		internal static TestStringReader GetReader() {
			return new TestStringReader("0*(3-5)/-9 -- line comment\r\n+1.0 /* block\r\ncomment */ *.0e2");
		}

		[Test]
		public void CheckLexicalError() {
			using (TestStringReader reader = new TestStringReader("1+x*200")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.LexicalError));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Error));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.End));
			}
		}

		[Test]
		public void BlockCommentWithInvalidInnerSymbol() {
			using (TestStringReader reader = new TestStringReader("/* don't */ 'do this'")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.CommentBlockRead));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.End));
			}
		}

		[Test]
		public void CheckTokens() {
			using (TestStringReader reader = GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("0"));
				Expect(token.Position.Index, EqualTo(0));
				Expect(token.Symbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("*"));
				Expect(token.Position.Index, EqualTo(1));
				Expect(token.Symbol.Name, EqualTo("*"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("("));
				Expect(token.Position.Index, EqualTo(2));
				Expect(token.Symbol.Name, EqualTo("("));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("3"));
				Expect(token.Position.Index, EqualTo(3));
				Expect(token.Symbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("-"));
				Expect(token.Position.Index, EqualTo(4));
				Expect(token.Symbol.Name, EqualTo("-"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("5"));
				Expect(token.Position.Index, EqualTo(5));
				Expect(token.Symbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(")"));
				Expect(token.Position.Index, EqualTo(6));
				Expect(token.Symbol.Name, EqualTo(")"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("/"));
				Expect(token.Position.Index, EqualTo(7));
				Expect(token.Symbol.Name, EqualTo("/"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("-"));
				Expect(token.Position.Index, EqualTo(8));
				Expect(token.Symbol.Name, EqualTo("-"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("9"));
				Expect(token.Position.Index, EqualTo(9));
				Expect(token.Symbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(" "));
				Expect(token.Position.Index, EqualTo(10));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.CommentLineRead));
				Expect(token.Text, EqualTo("-- line comment"));
				Expect(token.Position.Index, EqualTo(11));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("+"));
				Expect(token.Position.Index, EqualTo(28));
				Expect(token.Symbol.Name, EqualTo("+"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("1.0"));
				Expect(token.Position.Index, EqualTo(29));
				Expect(token.Symbol.Name, EqualTo("Float"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.CommentBlockRead));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("*"));
				Expect(token.Position.Index, EqualTo(54));
				Expect(token.Symbol.Name, EqualTo("*"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(".0e2"));
				Expect(token.Position.Index, EqualTo(55));
				Expect(token.Symbol.Name, EqualTo("Float"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.End));
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, null);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutReader() {
			new Tokenizer(null, grammar);
		}
	}
}
