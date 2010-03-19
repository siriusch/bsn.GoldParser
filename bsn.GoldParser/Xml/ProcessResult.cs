// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Xml {
	public struct ProcessResult {
		private readonly string errorMessage;
		private readonly LineInfo lineInfo;

		internal ProcessResult(LineInfo lineInfo, string errorMessage) {
			this.lineInfo = lineInfo;
			this.errorMessage = errorMessage;
		}

		public int Column {
			get {
				return lineInfo.Column;
			}
		}

		public int Line {
			get {
				return lineInfo.Line;
			}
		}

		public string Message {
			get {
				return errorMessage ?? string.Empty;
			}
		}

		public bool Success {
			get {
				return string.IsNullOrEmpty(errorMessage);
			}
		}

		public override string ToString() {
			if (Success) {
				return "Parsed successfully";
			}
			return string.Format("({0},{1}): {2}", Line, Column, errorMessage);
		}
	}
}