// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal sealed class LalrActionShift: LalrActionWithLalrState {
		public LalrActionShift(int index, Symbol symbol, LalrState state): base(index, symbol, state) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Shift;
			}
		}

		internal override TokenParseResult Execute(IParser parser, IToken token) {
			parser.PushTokenAndState(token, State);
			return TokenParseResult.Shift;
		}
	}
}