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

using NUnit.Framework;

namespace bsn.GoldParser.Parser {
	[TestFixture]
	public class TextBufferTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutTextReader() {
			new TextBuffer(null);
		}

		[Test]
		public void EmptyTextReaderLook() {
			using (TestStringReader reader = new TestStringReader(0)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				Expect(!charBuffer.TryLookahead(0, out ch));
			}
		}

		[Test]
		public void EmptyTextReaderRead() {
			using (TestStringReader reader = new TestStringReader(0)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				Expect(charBuffer.Read(0, out position) == string.Empty);
			}
		}

		[Test]
		public void MarkLongDistanceAndReplay() {
			using (TestStringReader reader = new TestStringReader(1024*20)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				LineInfo position;
				Expect(charBuffer.Read(128, out position) == reader.ToString().Substring(0, 128));
				int offset = 0;
				for (int i = 128; i < 1024*16; i++) {
					Expect(charBuffer.TryLookahead(ref offset, out ch));
					Expect(ch == reader[i]);
				}
				offset = 0;
				for (int i = 128; i < 1024*8; i++) {
					Expect(charBuffer.TryLookahead(ref offset, out ch));
					Expect(ch == reader[i]);
				}
				Expect(charBuffer.Read(reader.Length-128, out position) == reader.ToString().Substring(128));
				Expect(!charBuffer.TryLookahead(0, out ch));
			}
		}

		[Test]
		public void ReadManyChars() {
			using (TestStringReader reader = new TestStringReader(1024*1024)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				char ch;
				for (int i = 0; i < reader.Length; i++) {
					Expect(charBuffer.TryLookahead(i, out ch));
					Expect(ch == reader[i]);
				}
				Expect(!charBuffer.TryLookahead(reader.Length, out ch));
			}
		}

		[Test]
		public void ReadSingleChar() {
			using (TestStringReader reader = new TestStringReader(1)) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				Expect(charBuffer.Read(1, out position) == reader[0].ToString());
				char ch;
				Expect(!charBuffer.TryLookahead(0, out ch));
			}
		}

		[Test]
		public void MultilineCr() {
			using (TestStringReader reader = new TestStringReader("1\r\r3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Expect(position.Line, EqualTo(1));
				Expect(charBuffer.Line, EqualTo(3));
			}
		}

		[Test]
		public void MultilineLf() {
			using (TestStringReader reader = new TestStringReader("1\n\n3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Expect(position.Line, EqualTo(1));
				Expect(charBuffer.Line, EqualTo(3));
			}
		}

		[Test]
		public void MultilineLfCr() {
			using (TestStringReader reader = new TestStringReader("1\n\r\n\r3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Expect(position.Line, EqualTo(1));
				Expect(charBuffer.Line, EqualTo(3));
			}
		}

		[Test]
		public void MultilineCrLf() {
			using (TestStringReader reader = new TestStringReader("1\r\n\r\n3")) {
				TextBuffer charBuffer = new TextBuffer(reader);
				LineInfo position;
				charBuffer.Read(reader.Length, out position);
				Expect(position.Line, EqualTo(1));
				Expect(charBuffer.Line, EqualTo(3));
			}
		}
	}
}