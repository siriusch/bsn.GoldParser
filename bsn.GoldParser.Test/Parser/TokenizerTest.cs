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

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public class TokenizerTest {
		internal static TestStringReader GetReader() {
			return new TestStringReader("0*(3-5)/-9 -- line comment\r\n+1.0 /* block\r\ncomment */ *.0e2");
		}

		private readonly CompiledGrammar grammar;

		public TokenizerTest() {
			grammar = CgtCompiledGrammarTest.LoadCgtTestGrammar();
		}

		[Fact]
		public void BlockCommentWithNestedUnclosedString() {
			using (TestStringReader reader = new TestStringReader("/* don't /*** 'do ***/ this */ 0")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.CommentBlockRead, tokenizer.NextToken(out token));
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void BlockCommentWithUnclosedString() {
			using (TestStringReader reader = new TestStringReader("/* don't */ 'do this'")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.CommentBlockRead, tokenizer.NextToken(out token));
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void CheckLexicalErrorOnEnd() {
			using (TestStringReader reader = new TestStringReader("'")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.LexicalError, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Error, token.Symbol.Kind);
				Assert.Equal("'", token.Text);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void CheckMergedLexicalError() {
			using (TestStringReader reader = new TestStringReader("1+Nx*200")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				tokenizer.MergeLexicalErrors = true;
				Token token;
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.LexicalError, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Error, token.Symbol.Kind);
				Assert.Equal("Nx", token.Text);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void CheckTokens() {
			using (TestStringReader reader = GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("0", token.Text);
				Assert.Equal(0, token.Position.Index);
				Assert.Equal("Integer", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("*", token.Text);
				Assert.Equal(1, token.Position.Index);
				Assert.Equal("*", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("(", token.Text);
				Assert.Equal(2, token.Position.Index);
				Assert.Equal("(", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("3", token.Text);
				Assert.Equal(3, token.Position.Index);
				Assert.Equal("Integer", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("-", token.Text);
				Assert.Equal(4, token.Position.Index);
				Assert.Equal("-", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("5", token.Text);
				Assert.Equal(5, token.Position.Index);
				Assert.Equal("Integer", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(")", token.Text);
				Assert.Equal(6, token.Position.Index);
				Assert.Equal(")", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("/", token.Text);
				Assert.Equal(7, token.Position.Index);
				Assert.Equal("/", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("-", token.Text);
				Assert.Equal(8, token.Position.Index);
				Assert.Equal("-", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("9", token.Text);
				Assert.Equal(9, token.Position.Index);
				Assert.Equal("Integer", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(" ", token.Text);
				Assert.Equal(10, token.Position.Index);
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.CommentLineRead, tokenizer.NextToken(out token));
				Assert.Equal("-- line comment", token.Text);
				Assert.Equal(11, token.Position.Index);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("+", token.Text);
				Assert.Equal(28, token.Position.Index);
				Assert.Equal("+", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("1.0", token.Text);
				Assert.Equal(29, token.Position.Index);
				Assert.Equal("Float", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.CommentBlockRead, tokenizer.NextToken(out token));
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal("*", token.Text);
				Assert.Equal(54, token.Position.Index);
				Assert.Equal("*", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(".0e2", token.Text);
				Assert.Equal(55, token.Position.Index);
				Assert.Equal("Float", token.Symbol.Name);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void CheckUnmergedLexicalError() {
			using (TestStringReader reader = new TestStringReader("1+Nx*200")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.LexicalError, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Error, token.Symbol.Kind);
				Assert.Equal("N", token.Text);
				Assert.Equal(ParseMessage.LexicalError, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Error, token.Symbol.Kind);
				Assert.Equal("x", token.Text);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.End, token.Symbol.Kind);
			}
		}

		[Fact]
		public void ConstructWithoutGrammar() {
			using (TestStringReader reader = GetReader()) {
				Assert.Throws<ArgumentNullException>(() => {
					// ReSharper disable AccessToDisposedClosure
					new Tokenizer(reader, null);
					// ReSharper restore AccessToDisposedClosure
				});
			}
		}

		[Fact]
		public void ConstructWithoutReader() {
			Assert.Throws<ArgumentNullException>(() => {
				new Tokenizer(null, grammar);
			});
		}

		[Fact]
		public void EndOfDataWithUnfinishedTerminal() {
			using (TestStringReader reader = new TestStringReader("0 'zero")) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				Token token;
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.Terminal, token.Symbol.Kind);
				Assert.Equal(ParseMessage.TokenRead, tokenizer.NextToken(out token));
				Assert.Equal(SymbolKind.WhiteSpace, token.Symbol.Kind);
				Assert.Equal(ParseMessage.LexicalError, tokenizer.NextToken(out token));
			}
		}
	}
}
