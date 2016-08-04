// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
//
// This file has kindly been contributed by Jan Polášek
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
using System.IO;
using System.Text.RegularExpressions;

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute {
	public class Tests {
		private readonly CompiledGrammar grammar;

		public Tests() {
			grammar = EgtCompiledGrammarTest.LoadEgtTestGrammar();
		}

		[Fact]
		public void DynamicFactory() {
			SemanticTypeActions<DynamicMockTokenBase> actions = new SemanticTypeActions<DynamicMockTokenBase>(grammar);
			// ReSharper disable AccessToModifiedClosure
			Assert.Throws<InvalidOperationException>(() => actions.Initialize());
			// ReSharper restore AccessToModifiedClosure
			actions = new SemanticTypeActions<DynamicMockTokenBase>(grammar);
			try {
				actions.Initialize();
			} catch (InvalidOperationException exception) {
				Assert.True(Regex.IsMatch(exception.Message, "Rule .* is assigned to a non-static method, which is not allowed."));
			}
		}

		[Fact]
		public void GenericInitializes() {
			SemanticTypeActions<MockGenericTokenBase> actions = new SemanticTypeActions<MockGenericTokenBase>(grammar);
			actions.Initialize();
		}

		[Fact]
		public void GenericParse() {
			SemanticTypeActions<MockGenericTokenBase> actions = new SemanticTypeActions<MockGenericTokenBase>(grammar);
			actions.Initialize();
			SemanticProcessor<MockGenericTokenBase> processor = new SemanticProcessor<MockGenericTokenBase>(new StringReader("-1+2+3*4-8"), actions);
			Assert.Equal(ParseMessage.Accept, processor.ParseAll());
			Assert.IsAssignableFrom<MockGenericTokenBase>(processor.CurrentToken);
		}

		[Fact]
		public void Initializes() {
			SemanticTypeActions<MockTokenBase> actions = new SemanticTypeActions<MockTokenBase>(grammar);
			actions.Initialize();
		}
	}
}
