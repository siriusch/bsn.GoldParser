// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal sealed class LalrActionReduce: LalrAction {
		private readonly Rule reduceRule;

		public LalrActionReduce(int index, Symbol symbol, Rule reduceRule): base(index, symbol) {
			if (reduceRule == null) {
				throw new ArgumentNullException("reduceRule");
			}
			this.reduceRule = reduceRule;
		}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Reduce;
			}
		}

		public override object Target {
			get {
				return reduceRule;
			}
		}

		internal override TokenParseResult Execute<T>(IParser<T> parser, T token) {
			bool trim = reduceRule.ContainsOneNonterminal && parser.CanTrim(reduceRule);
			T head;
			if (trim) {
				head = parser.PopToken();
			} else {
				head = parser.CreateReduction(reduceRule);
			}
			LalrActionGoto gotoAction = parser.TopState.GetActionBySymbol(reduceRule.RuleSymbol) as LalrActionGoto;
			if (gotoAction == null) {
				Debug.Fail("Internal table error.");
				return TokenParseResult.InternalError;
			}
			parser.PushTokenAndState(head, gotoAction.State);
			return trim ? TokenParseResult.ReduceEliminated : TokenParseResult.ReduceNormal;
		}
	}
}