namespace bsn.GoldParser.Grammar {
	internal class LalrActionGoto: LalrActionWithLalrState {
		public LalrActionGoto(int index, Symbol symbol, LalrState state): base(index, symbol, state) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Goto;
			}
		}
	}
}