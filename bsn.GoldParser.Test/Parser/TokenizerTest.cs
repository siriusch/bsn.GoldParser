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
		public void BlockCommentWithNestedUnclosedString() {
			using (TestStringReader reader = new TestStringReader("/* don't /*** 'do ***/ this */ 0")) {
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
		public void BlockCommentWithUnclosedString() {
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
		public void CheckUnmergedLexicalError() {
			using (TestStringReader reader = new TestStringReader("1+xx*200")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.LexicalError));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Error));
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
		public void CheckMergedLexicalError() {
			using (TestStringReader reader = new TestStringReader("1+xx*200")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				tokenizer.MergeLexicalErrors = true;
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

		[Test]
		public void EndOfDataWithUnfinishedTerminal() {
			using (TestStringReader reader = new TestStringReader("0 'zero")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.Terminal));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.TokenRead));
				Expect(token.Symbol.Kind, EqualTo(SymbolKind.WhiteSpace));
				Expect(tokenizer.NextToken(out token), EqualTo(ParseMessage.LexicalError));
			}
		}
	}
}