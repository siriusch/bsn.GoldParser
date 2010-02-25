using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Xml {
	public struct ProcessResult {
		private readonly LineInfo lineInfo;
		private readonly string errorMessage;

		internal ProcessResult(LineInfo lineInfo, string errorMessage) {
			this.lineInfo = lineInfo;
			this.errorMessage = errorMessage;
		}

		public bool Success {
			get {
				return string.IsNullOrEmpty(errorMessage);
			}
		}

		public string Message {
			get {
				return errorMessage ?? string.Empty;
			}
		}

		public int Line {
			get {
				return lineInfo.Line;
			}
		}

		public int Column {
			get {
				return lineInfo.Column;
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