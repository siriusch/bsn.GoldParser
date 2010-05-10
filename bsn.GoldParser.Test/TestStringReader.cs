using System.IO;
using System.Text;

namespace bsn.GoldParser.Parser {
	internal class TestStringReader: StringReader {
		private readonly string data;

		private static string CreateTestString(int charCount) {
			StringBuilder builder = new StringBuilder(charCount);
			for (int i = 0; i < charCount; i++) {
				builder.Append((char)(i % 80 + 32));
			}
			return builder.ToString();
		}

		public TestStringReader(int charCount): this(CreateTestString(charCount)) {}

		public TestStringReader(string data): base(data) {
			this.data = data;
		}

		public char this[int index] {
			get {
				return data[index];
			}
		}

		public int Length {
			get {
				return data.Length;
			}
		}
	}
}