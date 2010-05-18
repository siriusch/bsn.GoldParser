// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal sealed class LalrActionGoto: LalrActionWithLalrState {
		public LalrActionGoto(int index, Symbol symbol, LalrState state): base(index, symbol, state) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Goto;
			}
		}

		internal override TokenParseResult Execute<T>(IParser<T> parser, T token) {
			return TokenParseResult.Empty;
		}
	}
}