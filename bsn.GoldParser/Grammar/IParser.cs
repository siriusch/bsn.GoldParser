// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	public interface IParser {
		Token TopToken {
			get;
		}

		bool CanTrim(Rule rule);
		Token CreateReduction(Rule rule);
		Token PopToken();
		void PushToken(Token token);
		void SetState(LalrState state);
	}
}