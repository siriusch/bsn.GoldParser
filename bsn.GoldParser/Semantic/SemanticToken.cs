using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticToken: Token {
		private readonly LineInfo position;
		private readonly Symbol symbol;

		protected SemanticToken(Symbol symbol, LineInfo position) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			this.symbol = symbol;
			this.position = position;
		}

		public override LineInfo Position {
			get {
				return position;
			}
		}

		public override Symbol Symbol {
			get {
				return symbol;
			}
		}
	}
}