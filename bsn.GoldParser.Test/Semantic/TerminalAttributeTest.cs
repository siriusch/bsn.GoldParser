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

using Xunit;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class TerminalAttributeTest {
		private readonly CompiledGrammar grammar;

		public TerminalAttributeTest() {
			grammar = EgtCompiledGrammarTest.LoadEgtTestGrammar();
		}

		[Fact]
		public void BindToGrammar() {
			TerminalAttribute terminalAttribute = new TerminalAttribute("Integer");
			Assert.NotNull(terminalAttribute.Bind(grammar));
		}

		[Fact]
		public void ConstructWithEmptyString() {
			Assert.Throws<ArgumentNullException>(() => {
				new TerminalAttribute(string.Empty);
			});
		}

		[Fact]
		public void ConstructWithGenericArgument() {
			new TerminalAttribute("Integer", typeof(TestValue));
		}

		[Fact]
		public void ConstructWithGenericArguments() {
			new TerminalAttribute("Integer", typeof(TestValue), typeof(TestValue));
		}

		[Fact]
		public void ConstructWithString() {
			new TerminalAttribute("Integer");
		}

		[Fact]
		public void ConstructWithoutString() {
			Assert.Throws<ArgumentNullException>(() => {
				new TerminalAttribute(null);
			});
		}
	}
}
