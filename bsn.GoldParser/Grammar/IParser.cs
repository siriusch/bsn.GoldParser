// (C) 2010 Arsène von Wyss / bsn
using System.Collections.Generic;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	public interface IParser {
		LalrState TopState {
			get;
		}

		bool CanTrim(Rule rule);
		Token CreateReduction(Rule rule);
		Token PopToken();
		void PushTokenAndState(Token token, LalrState state);
	}
}