// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
//
// This file has kinly been contributed by Jan Polasek
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

using NUnit.Framework;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute {
	[TestFixture]
	internal class Tests {
		private CompiledGrammar _grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			_grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		public void DynamicFactory() {
			SemanticTypeActions<DynamicMockTokenBase> actions = new SemanticTypeActions<DynamicMockTokenBase>(_grammar);
			Assert.Throws<InvalidOperationException>(actions.Initialize);
			actions = new SemanticTypeActions<DynamicMockTokenBase>(_grammar);
			try {
				actions.Initialize();
			} catch (InvalidOperationException exception) {
				Assert.True(Regex.IsMatch(exception.Message, "Rule .* is assigned to a non-static method, which is not allowed."));
			}
		}

		[Test]
		public void GenericInitializes() {
			SemanticTypeActions<MockGenericTokenBase> actions = new SemanticTypeActions<MockGenericTokenBase>(_grammar);
			Assert.DoesNotThrow(actions.Initialize);
		}

		[Test]
		public void GenericParse() {
			SemanticTypeActions<MockGenericTokenBase> actions = new SemanticTypeActions<MockGenericTokenBase>(_grammar);
			actions.Initialize();
			SemanticProcessor<MockGenericTokenBase> processor = new SemanticProcessor<MockGenericTokenBase>(new StringReader("-1+2+3*4-8"), actions);
			Assert.AreEqual(ParseMessage.Accept, processor.ParseAll());
			Assert.IsInstanceOf<MockGenericTokenBase>(processor.CurrentToken);
		}

		[Test]
		public void Initializes() {
			SemanticTypeActions<MockTokenBase> actions = new SemanticTypeActions<MockTokenBase>(_grammar);
			Assert.DoesNotThrow(actions.Initialize);
		}
	}
}
