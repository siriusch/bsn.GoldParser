using System;
using System.IO;

using bsn.GoldParser.Grammar;

using NUnit.Framework;

namespace bsn.GoldParser.Parser {
	[TestFixture]
	public class TokenizerTest: AssertionHelper {
		internal static TestStringReader GetReader() {
			return new TestStringReader("0*(3-5)/-9 -- line comment\r\n+1.0 /* block\r\ncomment */ *.0e2");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutReader() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			new Tokenizer(null, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutInitialState() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, null, grammar.EndSymbol, grammar.ErrorSymbol);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutEndSymbol() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, grammar.DfaInitialState, null, grammar.ErrorSymbol);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ConstructWithInvalidEndSymbol() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, grammar.DfaInitialState, grammar.ErrorSymbol, grammar.ErrorSymbol);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutErrorSymbol() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, null);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ConstructWithInvalidErrorSymbol() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.EndSymbol);
			}
		}

		[Test]
		public void CheckLexicalError() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = new TestStringReader("1+x*200")) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
				TextToken token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.LexicalError));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.Error));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.End));
			}
		}

		[Test]
		public void CheckTokens() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			using (TestStringReader reader = GetReader()) {
				ITokenizer tokenizer = new Tokenizer(reader, grammar.DfaInitialState, grammar.EndSymbol, grammar.ErrorSymbol);
				TextToken token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("0"));
				Expect(token.Position.Index, EqualTo(0));
				Expect(token.ParentSymbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("*"));
				Expect(token.Position.Index, EqualTo(1));
				Expect(token.ParentSymbol.Name, EqualTo("*"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("("));
				Expect(token.Position.Index, EqualTo(2));
				Expect(token.ParentSymbol.Name, EqualTo("("));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("3"));
				Expect(token.Position.Index, EqualTo(3));
				Expect(token.ParentSymbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("-"));
				Expect(token.Position.Index, EqualTo(4));
				Expect(token.ParentSymbol.Name, EqualTo("-"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("5"));
				Expect(token.Position.Index, EqualTo(5));
				Expect(token.ParentSymbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(")"));
				Expect(token.Position.Index, EqualTo(6));
				Expect(token.ParentSymbol.Name, EqualTo(")"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("/"));
				Expect(token.Position.Index, EqualTo(7));
				Expect(token.ParentSymbol.Name, EqualTo("/"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("-"));
				Expect(token.Position.Index, EqualTo(8));
				Expect(token.ParentSymbol.Name, EqualTo("-"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("9"));
				Expect(token.Position.Index, EqualTo(9));
				Expect(token.ParentSymbol.Name, EqualTo("Integer"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(" "));
				Expect(token.Position.Index, EqualTo(10));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.CommentLineRead));
				Expect(token.Text, EqualTo("-- line comment"));
				Expect(token.Position.Index, EqualTo(11));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("+"));
				Expect(token.Position.Index, EqualTo(28));
				Expect(token.ParentSymbol.Name, EqualTo("+"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("1.0"));
				Expect(token.Position.Index, EqualTo(29));
				Expect(token.ParentSymbol.Name, EqualTo("Float"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.CommentBlockRead));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo("*"));
				Expect(token.Position.Index, EqualTo(54));
				Expect(token.ParentSymbol.Name, EqualTo("*"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Text, EqualTo(".0e2"));
				Expect(token.Position.Index, EqualTo(55));
				Expect(token.ParentSymbol.Name, EqualTo("Float"));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.ParentSymbol.Kind, EqualTo(SymbolKind.End));
			}
		}
	}
}