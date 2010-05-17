using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class OperatorToken: SemanticToken {
		private readonly string op;

		public OperatorToken(TextToken token): base(token.Symbol, token.Position) {
			this.op = token.Text;
		}

		public string Op {
			get {
				return op;
			}
		}
	}
}
