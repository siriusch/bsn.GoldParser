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
	public class CharBufferTest: AssertionHelper {
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructWithoutTextReader() {
			new CharBuffer(null);
		}

		[Test]
		public void EmptyTextReader() {
			using (TestStringReader reader = new TestStringReader(0)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}

		[Test]
		public void MarkCreate() {
			using (TestStringReader reader = new TestStringReader(0)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				using (CharBuffer.Mark mark = charBuffer.CreateMark()) {
					Expect(mark != null);
				}
			}
		}

		[Test]
		public void MarkLongDistanceAndReplay() {
			using (TestStringReader reader = new TestStringReader(1024*20)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				for (int i = 0; i < 128; i++) {
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == reader[i]);
				}
				using (CharBuffer.Mark mark = charBuffer.CreateMark()) {
					Expect(mark.Distance == 0);
					for (int i = 128; i < 1024*16; i++) {
						Expect(charBuffer.TryReadChar(out ch));
						Expect(ch == reader[i]);
					}
					Expect(mark.Distance == (1024*16-128));
					charBuffer.MoveToMark(mark);
					Expect(mark.Distance == 0);
					for (int i = 128; i < 1024*8; i++) {
						Expect(charBuffer.TryReadChar(out ch));
						Expect(ch == reader[i]);
					}
				}
				for (int i = 1024*8; i < reader.Length; i++) {
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == reader[i]);
				}
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}

		[Test]
		public void MarkShortDistance() {
			using (TestStringReader reader = new TestStringReader(2)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				using (CharBuffer.Mark mark = charBuffer.CreateMark()) {
					Expect(mark.Distance == 0);
					char c;
					Expect(charBuffer.TryReadChar(out c));
					Expect(mark.Distance == 1);
					charBuffer.MoveToMark(mark);
					Expect(mark.Distance == 0);
				}
			}
		}

		[Test]
		public void MarkShortReplay() {
			using (TestStringReader reader = new TestStringReader(10)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				using (CharBuffer.Mark mark = charBuffer.CreateMark()) {
					Expect(mark != null);
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == reader[0]);
					charBuffer.MoveToMark(mark);
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == reader[0]);
				}
			}
		}

		[Test]
		public void ReadManyChars() {
			using (TestStringReader reader = new TestStringReader(1024*1024)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				for (int i = 0; i < reader.Length; i++) {
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == reader[i]);
				}
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}

		[Test]
		public void ReadSingleChar() {
			using (TestStringReader reader = new TestStringReader(1)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				Expect(charBuffer.TryReadChar(out ch));
				Expect(ch == reader[0]);
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}
	}
}