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
			this.symbol = symbol;
			this.position = position;
		}

		LineInfo IToken.Position {
			get {
				return position;
			}
		}

		bool IToken.NameIs(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			return (symbol != null) && name.Equals(symbol.Name, StringComparison.Ordinal);
		}

		Symbol IToken.Symbol {
			get {
				return symbol;
			}
		}
	}
}
