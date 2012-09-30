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
using System.Globalization;

using Xunit;

namespace bsn.GoldParser.Parser {
	public class TextBufferTest {
		[Fact]
		public void ConstructWithoutTextReader() {
			Assert.Throws<ArgumentNullException>(() => {
				new TextBuffer(null);
			});
		}

		[Fact]
		public void EmptyTextReaderLook() {
			using (TestStringReader reader = new TestStringReader(0)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				Assert.False(charBuffer.TryLookahead(0, out ch));
			}
		}

		[Fact]
		public void EmptyTextReaderRead() {
			using (TestStringReader reader = new TestStringReader(0)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				Assert.Equal(string.Empty, charBuffer.Read(0, out position));
			}
		}

		[Fact]
		public void MarkLongDistanceAndReplay() {
			using (TestStringReader reader = new TestStringReader(1024*20)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				LineInfo position;
				Assert.Equal(reader.ToString().Substring(0, 128), charBuffer.Read(128, out position));
				int offset = 0;
				for (int i = 128; i < 1024*16; i++) {
					Assert.True(charBuffer.TryLookahead(ref offset, out ch));
					Assert.Equal(reader[i], ch);
				}
				offset = 0;
				for (int i = 128; i < 1024*8; i++) {
					Assert.True(charBuffer.TryLookahead(ref offset, out ch));
					Assert.Equal(reader[i], ch);
				}
				Assert.Equal(reader.ToString().Substring(128), charBuffer.Read(reader.Length-128, out position));
				Assert.False(charBuffer.TryLookahead(0, out ch));
			}
		}

		[Fact]
		public void ReadAndRollback() {
			using (TestStringReader reader = new TestStringReader(1024*20)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position1;
				Assert.Equal(reader.ToString().Substring(0, 1280), charBuffer.Read(1280, out position1));
				Assert.Equal(charBuffer.Position, 1280);
				charBuffer.Rollback();
				Assert.Equal(charBuffer.Position, 0);
				LineInfo position2;
				Assert.Equal(reader.ToString().Substring(0, 1280), charBuffer.Read(1280, out position2));
				Assert.Equal(position1, position2);
			}
		}

		[Fact]
		public void MultilineCr() {
			using (TestStringReader reader = new TestStringReader("1\r\r3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Assert.Equal(1, position.Line);
				Assert.Equal(3, charBuffer.Line);
			}
		}

		[Fact]
		public void MultilineCrLf() {
			using (TestStringReader reader = new TestStringReader("1\r\n\r\n3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Assert.Equal(1, position.Line);
				Assert.Equal(3, charBuffer.Line);
			}
		}

		[Fact]
		public void MultilineLf() {
			using (TestStringReader reader = new TestStringReader("1\n\n3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Assert.Equal(1, position.Line);
				Assert.Equal(3, charBuffer.Line);
			}
		}

		[Fact]
		public void MultilineLfCr() {
			using (TestStringReader reader = new TestStringReader("1\n\r\n\r3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Assert.Equal(1, position.Line);
				Assert.Equal(3, charBuffer.Line);
			}
		}

		[Fact]
		public void ReadManyChars() {
			using (TestStringReader reader = new TestStringReader(1024*1024)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				for (int i = 0; i < reader.Length; i++) {
					Assert.True(charBuffer.TryLookahead(i, out ch));
					Assert.Equal(reader[i], ch);
				}
				Assert.False(charBuffer.TryLookahead(reader.Length, out ch));
			}
		}

		[Fact]
		public void ReadSingleChar() {
			using (TestStringReader reader = new TestStringReader(1)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				Assert.Equal(reader[0].ToString(CultureInfo.InvariantCulture), charBuffer.Read(1, out position));
				char ch;
				Assert.False(charBuffer.TryLookahead(0, out ch));
			}
		}
	}
}
