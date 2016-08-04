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

using bsn.GoldParser.Grammar;

using Xunit;

namespace bsn.GoldParser.Semantic.StrictCtorParameterMatching {
	public class ParameterMatching {
		private readonly CompiledGrammar grammar;

		public ParameterMatching() {
			grammar = EgtCompiledGrammarTest.LoadEgtTestGrammar();
		}

		[Fact]
		public void BaseTest() {
			SemanticTypeActions<MockTokenBase> actionsA = new SemanticTypeActions<MockTokenBase>(grammar);
			Assert.Throws<InvalidOperationException>(() => actionsA.Initialize(true, true));
			SemanticTypeActions<MockTokenBase> actionsB = new SemanticTypeActions<MockTokenBase>(grammar);
			actionsB.Initialize(true, false);
			//default should be false
			SemanticTypeActions<MockTokenBase> actionsC = new SemanticTypeActions<MockTokenBase>(grammar);
			actionsC.Initialize(true);
		}

		[Fact]
		public void ExplicitCheckTest() {
			SemanticTypeActions<MockTokenBaseExplicitChecks> actionsA = new SemanticTypeActions<MockTokenBaseExplicitChecks>(grammar);
			Assert.Throws<InvalidOperationException>(() => actionsA.Initialize(true, true));
			SemanticTypeActions<MockTokenBaseExplicitChecks> actionsB = new SemanticTypeActions<MockTokenBaseExplicitChecks>(grammar);
			Assert.Throws<InvalidOperationException>(() => actionsB.Initialize(true, false));
		}

		[Fact]
		public void ExplicitNoCheckTest() {
			SemanticTypeActions<MockTokenBaseExplicitNoChecks> actionsA = new SemanticTypeActions<MockTokenBaseExplicitNoChecks>(grammar);
			actionsA.Initialize(true, true);
			SemanticTypeActions<MockTokenBaseExplicitNoChecks> actionsB = new SemanticTypeActions<MockTokenBaseExplicitNoChecks>(grammar);
			actionsB.Initialize(true, false);
		}
	}
}
