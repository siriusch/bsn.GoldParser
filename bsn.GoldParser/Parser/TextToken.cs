// (C) 2010 Arsène von Wyss / bsn
using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Represents data about current token.
	/// </summary>
	public sealed class TextToken: Token {
		private readonly Symbol symbol; // Token symbol.
		private readonly LineInfo position; // Token source line number.
		private readonly string text; // Token text.

		internal TextToken(Symbol symbol, string text, LineInfo position) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (text == null) {
				throw new ArgumentNullException("text");
			}
			this.symbol = symbol;
			this.position = position;
			this.text = this.symbol.Name.Equals(text, StringComparison.OrdinalIgnoreCase) ? this.symbol.Name : text; // "intern" the strings which are equal to the terminal name
		}

		public override Symbol Symbol {
			get {
				return symbol;
			}
		}

		public override LineInfo Position {
			get {
				return position;
			}
		}

		public override string Text {
			get {
				return text;
			}
		}

		public override string ToString() {
			return Text;
		}
	}
}