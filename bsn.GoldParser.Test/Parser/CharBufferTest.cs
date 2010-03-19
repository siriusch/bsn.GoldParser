// (C) 2010 Arsène von Wyss / bsn
using System;
using System.IO;
using System.Text;

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
			using (TextReader reader = new StringReader(string.Empty)) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}

		[Test]
		public void ReadSingleChar() {
			using (TextReader reader = new StringReader("1")) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				Expect(charBuffer.TryReadChar(out ch));
				Expect(ch == '1');
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}

		[Test]
		public void ReadManyChars() {
			const int charCount = 1024*1024;
			StringBuilder builder = new StringBuilder(charCount);
			for (int i = 0; i < charCount; i++) {
				builder.Append((char)(i % 80 + 32));
			}
			using (TextReader reader = new StringReader(builder.ToString())) {
				CharBuffer charBuffer = new CharBuffer(reader);
				char ch;
				for (int i = 0; i < charCount; i++) {
					Expect(charBuffer.TryReadChar(out ch));
					Expect(ch == builder[i]);
				}
				Expect(!charBuffer.TryReadChar(out ch));
			}
		}
	}
}