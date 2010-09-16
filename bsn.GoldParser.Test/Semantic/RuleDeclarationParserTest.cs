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
using System.Diagnostics;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class RuleDeclarationParserTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = CompiledGrammarTest.LoadTestGrammar();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutGrammar() {
			new RuleDeclarationParser(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullRuleString() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			parser.TryParse(null, out rule);
		}

		[Test]
		public void ValidRuleStringComplex() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Value> ::= '(' <Expression> ')'", out rule), True);
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleMap1() {
			Reduction reduction;
			Expect(RuleDeclarationParser.TryParse("<Value> ::= ~'(' <Expression> ~')'", out reduction), True);
			foreach (int index in RuleDeclarationParser.GetRuleHandleIndexes(reduction)) {
				Debug.WriteLine(index);
			}
		}

		[Test]
		public void ValidRuleMap2() {
			Reduction reduction;
			Expect(RuleDeclarationParser.TryParse("<Value> ::= ~'(' 1:<Expression> ')'", out reduction), True);
			foreach (int index in RuleDeclarationParser.GetRuleHandleIndexes(reduction)) {
				Debug.WriteLine(index);
			}
		}

		[Test]
		public void ValidRuleStringEmpty() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Empty> ::=", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}

		[Test]
		public void ValidRuleStringSimple() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Expect(parser.TryParse("<Value> ::= Float", out rule), EqualTo(true));
			Debug.WriteLine(rule.Definition);
		}
	}
}