// $Archive: /BU-HSE/SP-HSED/Code/Sirius2.GoldParser/TextToken.cs $
// $Revision: 5 $
// $UTCDate: 2009-05-07 09:43:35Z $
// $Author: vonwyssa $
// 
// (C) Sirius Technologies AG, Basel. - $NoKeywords:  $
using System;
using System.Diagnostics;
using System.Text;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Represents data about current token.
	/// </summary>
	public sealed class TextToken: Token {
		private readonly LineInfo position; // Token source line number.
		private readonly Symbol parentSymbol; // Token symbol.
		private readonly string text; // Token text.

		internal TextToken(Symbol symbol, string text, LineInfo position) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (text == null) {
				throw new ArgumentNullException("text");
			}
			parentSymbol = symbol;
			this.position = position;
			this.text = parentSymbol.Name.Equals(text, StringComparison.OrdinalIgnoreCase) ? parentSymbol.Name : text; // "intern" the strings which are equal to the terminal name
		}

		public override LineInfo Position {
			get {
				return position;
			}
		}

		public override Symbol ParentSymbol {
			get {
				return parentSymbol;
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