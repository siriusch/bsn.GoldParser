// (C) 2010 Arsène von Wyss / bsn
using System;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// The <c>Token</c> class is the non-generic base class for tokens in the AST (abstract syntax tree).
	/// </summary>
	public abstract class Token {
		private LalrState state;

		public virtual Token[] Children {
			get {
				return new Token[0];
			}
		}

		/// <summary>
		/// Gets the symbol associated with this token.
		/// </summary>
		/// <value>The parent symbol.</value>
		public abstract Symbol ParentSymbol {
			get;
		}

		/// <summary>
		/// Gets the line number where this token begins.
		/// </summary>
		/// <value>The line number and position.</value>
		public abstract LineInfo Position {
			get;
		}

		public virtual string Text {
			get {
				return string.Empty;
			}
		}

		internal LalrState State {
			get {
				return state;
			}
			set {
				state = value;
			}
		}
	}
}