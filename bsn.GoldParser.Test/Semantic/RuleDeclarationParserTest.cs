﻿// bsn GoldParser .NET Engine
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

using Xunit;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class RuleDeclarationParserTest {
		private readonly CompiledGrammar grammar;

		public RuleDeclarationParserTest() {
			grammar = CgtCompiledGrammarTest.LoadCgtTestGrammar();
		}

		[Fact]
		public void ConstructWithoutGrammar() {
			Assert.Throws<ArgumentNullException>(() => {
				new RuleDeclarationParser(null);
			});
		}

		[Fact]
		public void NullRuleString() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Assert.Throws<ArgumentNullException>(() => {
				parser.TryParse(null, out rule);
			});
		}

		[Fact]
		public void ValidRuleMap1() {
			Reduction reduction;
			Assert.True(RuleDeclarationParser.TryParse("<Value> ::= ~'(' <Expression> ~')'", out reduction));
			foreach (int index in RuleDeclarationParser.GetRuleHandleIndexes(reduction)) {
				Debug.WriteLine(index);
			}
		}

		[Fact]
		public void ValidRuleMap2() {
			Reduction reduction;
			Assert.True(RuleDeclarationParser.TryParse("<Value> ::= ~'(' 1:<Expression> ')'", out reduction));
			foreach (int index in RuleDeclarationParser.GetRuleHandleIndexes(reduction)) {
				Debug.WriteLine(index);
			}
		}

		[Fact]
		public void ValidRuleStringComplex() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Assert.True(parser.TryParse("<Value> ::= '(' <Expression> ')'", out rule));
			Debug.WriteLine(rule.Definition);
		}

		[Fact]
		public void ValidRuleStringEmpty() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Assert.Equal(true, parser.TryParse("<Empty> ::=", out rule));
			Debug.WriteLine(rule.Definition);
		}

		[Fact]
		public void ValidRuleStringSimple() {
			RuleDeclarationParser parser = new RuleDeclarationParser(grammar);
			Rule rule;
			Assert.Equal(true, parser.TryParse("<Value> ::= Float", out rule));
			Debug.WriteLine(rule.Definition);
		}
	}
}
