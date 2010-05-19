using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticToken: IToken {
		private LineInfo position;
		private Symbol symbol;

		protected SemanticToken() {}

		protected internal virtual void Initialize(Symbol symbol, LineInfo position) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
		}

		LineInfo IToken.Position {
			get {
				return position;
			}
		}

		Symbol IToken.Symbol {
			get {
				return symbol;
			}
		}
	}
}
