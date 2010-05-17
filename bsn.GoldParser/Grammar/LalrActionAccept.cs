// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal sealed class LalrActionAccept: LalrAction {
		public LalrActionAccept(int index, Symbol symbol): base(index, symbol) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Accept;
			}
		}

		internal override TokenParseResult Execute(IParser parser, Token token) {
			return TokenParseResult.Accept;
		}
	}
}