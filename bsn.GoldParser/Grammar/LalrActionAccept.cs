// (C) 2010 Ars�ne von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal sealed class LalrActionAccept: LalrAction {
		public LalrActionAccept(int index, Symbol symbol): base(index, symbol) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Accept;
			}
		}

		internal override TokenParseResult Execute<T>(IParser<T> parser, T token) {
			return TokenParseResult.Accept;
		}
	}
}