// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Xml {
	/// <summary>
	/// A structure to return the parsing result of a <see cref="GrammarXmlProcessor"/>
	/// </summary>
	public struct ProcessResult {
		private readonly string errorMessage;
		private readonly LineInfo lineInfo;

		internal ProcessResult(LineInfo lineInfo, string errorMessage) {
			this.lineInfo = lineInfo;
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Gets the text column.
		/// </summary>
		/// <value>The text column.</value>
		public int Column {
			get {
				return lineInfo.Column;
			}
		}

		/// <summary>
		/// Gets the text line.
		/// </summary>
		/// <value>The text line.</value>
		public int Line {
			get {
				return lineInfo.Line;
			}
		}

		/// <summary>
		/// Gets the parser error message.
		/// </summary>
		/// <value>The parser message.</value>
		public string Message {
			get {
				return errorMessage ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the processing was terminated successfully.
		/// </summary>
		/// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
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