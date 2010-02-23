using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal class LalrActionShift: LalrActionWithLalrState {
		public LalrActionShift(int index, Symbol symbol, LalrState state): base(index, symbol, state) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Shift;
			}
		}

		internal override TokenParseResult Execute(IParser parser, Token token) {
			token.State = State;
			parser.PushToken(token);
			parser.SetState(State);
			return TokenParseResult.Shift;
		}
	}
}