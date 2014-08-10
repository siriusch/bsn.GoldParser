// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2014 by Arsène von Wyss - avw@gmx.ch
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
using System.Drawing;

using Xunit;
using Xunit.Extensions;

namespace bsn.GoldParser.Text {
	public class ConsoleTextWriterTest {
		[Theory]
		[InlineData(ConsoleColor.Black, KnownColor.Black)]
		[InlineData(ConsoleColor.Blue, KnownColor.Blue)]
		[InlineData(ConsoleColor.Cyan, KnownColor.Cyan)]
		[InlineData(ConsoleColor.DarkBlue, KnownColor.DarkBlue)]
		[InlineData(ConsoleColor.DarkCyan, KnownColor.DarkCyan)]
		[InlineData(ConsoleColor.DarkGray, KnownColor.Gray)]
		[InlineData(ConsoleColor.DarkGreen, KnownColor.Green)]
		[InlineData(ConsoleColor.DarkMagenta, KnownColor.DarkMagenta)]
		[InlineData(ConsoleColor.DarkRed, KnownColor.DarkRed)]
		[InlineData(ConsoleColor.DarkYellow, KnownColor.Olive)]
		[InlineData(ConsoleColor.Gray, KnownColor.LightGray)]
		[InlineData(ConsoleColor.Green, KnownColor.Lime)]
		[InlineData(ConsoleColor.Magenta, KnownColor.Magenta)]
		[InlineData(ConsoleColor.Red, KnownColor.Red)]
		[InlineData(ConsoleColor.White, KnownColor.White)]
		[InlineData(ConsoleColor.Yellow, KnownColor.Yellow)]
		public void ColorMapping(ConsoleColor expexted, KnownColor input) {
			Assert.Equal(expexted, ConsoleTextWriter.GetConsoleColor(Color.FromKnownColor(input)));
		}
	}
}
