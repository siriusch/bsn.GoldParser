using System;

using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// Action in a LR State. 
	/// </summary>
	public abstract class LalrAction: GrammarObject {
		private readonly Symbol symbol;

		/// <summary>
		/// Creats a new instance of the <c>LalrAction</c> class.
		/// </summary>
		/// <param name="index">Index of the LR state action.</param>
		/// <param name="symbol">Symbol associated with the action.</param>
		internal LalrAction(int index, Symbol symbol): base(symbol.Owner, index) {
			this.symbol = symbol;
		}

		/// <summary>
		/// Gets action type.
		/// </summary>
		public abstract LalrActionType ActionType {
			get;
		}

		/// <summary>
		/// Gets symbol associated with the LR state action.
		/// </summary>
		public Symbol Symbol {
			get {
				return symbol;
			}
		}

		public virtual object Target {
			get {
				return null;
			}
		}

		public override string ToString() {
			return string.Format("Action {0}: {1}, Symbol {2}, Target: {3}", Index, ActionType, Symbol, Target);
		}

		internal abstract TokenParseResult Execute(IParser parser, Token token);
	}
}