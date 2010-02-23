using System;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal abstract class LalrActionWithLalrState: LalrAction {
		private readonly LalrState state;

		protected LalrActionWithLalrState(int index, Symbol symbol, LalrState state): base(index, symbol) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			this.state = state;
		}

		public LalrState State {
			get {
				return state;
			}
		}

		internal override TokenParseResult Execute(IParser parser, Token token) {
			return TokenParseResult.Empty;
		}
	}
}