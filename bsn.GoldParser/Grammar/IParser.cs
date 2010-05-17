// (C) 2010 Ars�ne von Wyss / bsn
using System.Collections.Generic;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	public interface IParser {
		LalrState TopState {
			get;
		}

		bool CanTrim(Rule rule);
		IToken CreateReduction(Rule rule);
		IToken PopToken();
		void PushTokenAndState(IToken token, LalrState state);
	}
}