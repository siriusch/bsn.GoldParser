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
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticProcessorTest {
		private readonly SemanticTypeActions<TestToken> actions;

		public SemanticProcessorTest() {
			actions = new SemanticTypeActions<TestToken>(CompiledGrammarTest.LoadTestGrammar());
			actions.Initialize(false);
		}

		[Fact]
		public void ParseComplexExpression() {
			using (TestStringReader reader = new TestStringReader("((100+5.0)/\r\n(4.5+.5))-\r\n12345.4e+1")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Assert.Equal(ParseMessage.Accept, processor.ParseAll());
				Assert.IsAssignableFrom<TestValue>(processor.CurrentToken);
				TestValue value = (TestValue)processor.CurrentToken;
				Assert.Equal(-123433.0, value.Compute());
			}
		}

		[Fact]
		public void ParseEmpty() {
			using (TestStringReader reader = new TestStringReader("")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Assert.Equal(ParseMessage.Accept, processor.ParseAll());
				Assert.IsType<TestEmpty>(processor.CurrentToken);
			}
		}

		[Fact]
		public void ParseNull() {
			using (TestStringReader reader = new TestStringReader("NULL")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Assert.Equal(ParseMessage.Accept, processor.ParseAll());
				Assert.IsType<TestEmpty>(processor.CurrentToken);
			}
		}

		[Fact]
		public void ParseSimpleExpression() {
			using (TestStringReader reader = new TestStringReader("100")) {
				SemanticProcessor<TestToken> processor = new SemanticProcessor<TestToken>(reader, actions);
				Assert.Equal(ParseMessage.Accept, processor.ParseAll());
				Assert.IsAssignableFrom<TestValue>(processor.CurrentToken);
				TestValue value = (TestValue)processor.CurrentToken;
				Assert.Equal(100, value.Compute());
			}
		}
	}
}
