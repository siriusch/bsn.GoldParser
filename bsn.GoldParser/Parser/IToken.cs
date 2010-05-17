using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public interface IToken {
		/// <summary>
		/// Gets the symbol associated with this token.
		/// </summary>
		/// <value>The parent symbol.</value>
		Symbol Symbol {
			get;
		}

		/// <summary>
		/// Gets the line number where this token begins.
		/// </summary>
		/// <value>The line number and position.</value>
		LineInfo Position {
			get;
		}
	}
}