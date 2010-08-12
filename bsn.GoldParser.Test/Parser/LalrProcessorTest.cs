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
	public class LalrProcessorTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
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

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutTokenizer() {
			new LalrProcessor(null);
		}

		[Test]
		public void ParseAll() {
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
				LalrProcessor processor = new LalrProcessor(tokenizer, true);
				Expect(processor.ParseAll(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken.Symbol.Name, EqualTo("Expression"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(27));
			}
		}

		[Test]
		public void ParseTreeWithTrim() {
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
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
			using (TestStringReader reader = TokenizerTest.GetReader()) {
				Tokenizer tokenizer = new Tokenizer(reader, grammar);
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
				Expect(processor.Parse(), EqualTo(ParseMessage.Reduction));
				Expect(processor.CurrentToken.GetType(), EqualTo(typeof(Reduction)));
				Expect(processor.Parse(), EqualTo(ParseMessage.Accept));
				Expect(processor.CurrentToken.Symbol.Name, EqualTo("Root"));
				Expect(CountTokens(processor.CurrentToken), EqualTo(40));
			}
		}
	}
}