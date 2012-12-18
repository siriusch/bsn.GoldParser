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

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public class LalrProcessorTest {
		private readonly CompiledGrammar grammar;

		public LalrProcessorTest() {
			grammar = CgtCompiledGrammarTest.LoadCgtTestGrammar();
		}

		[Fact]
		public void ConstructWithoutTokenizer() {
			Assert.Throws<ArgumentNullException>(() => {
				new LalrProcessor(null);
			});
		}

		[Fact]
		public void ParseAll() {
			using (TestStringReader reader = CgtTokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer, true);
				Assert.Equal(ParseMessage.Accept, processor.ParseAll());
				Assert.Equal("Expression", processor.CurrentToken.Symbol.Name);
				Assert.Equal(27, CountTokens(processor.CurrentToken));
			}
		}

		[Fact]
		public void ParseTreeWithTrim() {
			using (TestStringReader reader = CgtTokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer, true);
				Assert.Equal(true, processor.Trim);
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.CommentLineRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.CommentBlockRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Accept, processor.Parse());
				Assert.Equal("Expression", processor.CurrentToken.Symbol.Name);
				Assert.Equal(27, CountTokens(processor.CurrentToken));
			}
		}

		[Fact]
		public void ParseTreeWithoutTrim() {
			using (TestStringReader reader = CgtTokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer);
				Assert.Equal(false, processor.Trim);
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.CommentLineRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.CommentBlockRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.TokenRead, processor.Parse());
				Assert.Equal(typeof(TextToken), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Reduction, processor.Parse());
				Assert.Equal(typeof(Reduction), processor.CurrentToken.GetType());
				Assert.Equal(ParseMessage.Accept, processor.Parse());
				Assert.Equal("Root", processor.CurrentToken.Symbol.Name);
				Assert.Equal(40, CountTokens(processor.CurrentToken));
			}
		}

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
	}
}
