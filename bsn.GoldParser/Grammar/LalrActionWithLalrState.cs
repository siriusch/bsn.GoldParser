// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal abstract class LalrActionWithLalrState: LalrAction {
		private readonly LalrState state;

		protected LalrActionWithLalrState(int index, Symbol symbol, LalrState state): base(index, symbol) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			Debug.Assert(symbol.Owner == state.Owner);
			this.state = state;
		}

		public LalrState State {
			get {
				return state;
			}
		}
	}
}