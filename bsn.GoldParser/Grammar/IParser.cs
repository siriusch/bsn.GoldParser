// (C) 2010 Arsène von Wyss / bsn
using System.Collections.Generic;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	public interface IParser<T> where T: IToken {
		LalrState TopState {
			get;
		}

		bool CanTrim(Rule rule);
		T CreateReduction(Rule rule);
		T PopToken();
		void PushTokenAndState(T token, LalrState state);
	}
}