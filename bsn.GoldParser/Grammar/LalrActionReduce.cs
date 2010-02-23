using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	internal class LalrActionReduce: LalrAction {
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

		internal override TokenParseResult Execute(IParser parser, Token token) {
			bool trim = reduceRule.ContainsOneNonTerminal && parser.CanTrim(reduceRule);
			Token head;
			if (trim) {
				head = parser.PopToken();
			} else {
				head = parser.CreateReduction(reduceRule);
			}
			LalrActionGoto gotoAction = parser.TopToken.State.GetActionBySymbol(reduceRule.Head) as LalrActionGoto;
			if (gotoAction == null) {
				Debug.Fail("Internal table error.");
				return TokenParseResult.InternalError;
			}
			head.State = gotoAction.State;
			parser.PushToken(head);
			parser.SetState(gotoAction.State);
			return trim ? TokenParseResult.ReduceEliminated : TokenParseResult.ReduceNormal;
		}
	}
}