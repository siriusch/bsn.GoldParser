// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Globalization;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// A structure holding information about the text position of a specific token.
	/// </summary>
	public struct LineInfo: IEquatable<LineInfo>, IComparable<LineInfo> {
		private readonly int column;
		private readonly int line;
		private readonly int index;

		/// <summary>
		/// Initializes a new instance of the <see cref="LineInfo"/> struct.
		/// </summary>
		/// <param name="index">The character index.</param>
		/// <param name="line">The line.</param>
		/// <param name="column">The column.</param>
		public LineInfo(int index, int line, int column) {
			this.line = line;
			this.index = index;
			this.column = column;
		}

		/// <summary>
		/// Gets the character index.
		/// </summary>
		/// <value>The character index.</value>
		public int Index {
			get {
				return index;
			}
		}

		/// <summary>
		/// Gets the column.
		/// </summary>
		/// <value>The column.</value>
		public int Column {
			get {
				return column;
			}
		}

		/// <summary>
		/// Gets the line.
		/// </summary>
		/// <value>The line.</value>
		public int Line {
			get {
				return line;
			}
		}

		public bool Equals(LineInfo other) {
			return (other.line == line) && (other.column == column) && (other.index == column);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (obj.GetType() != typeof(LineInfo)) {
				return false;
			}
			return Equals((LineInfo)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (line*397)^(column*31)^index;
			}
		}

		public int CompareTo(LineInfo other) {
			return index-other.index;
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", line, column);
		}
	}
}