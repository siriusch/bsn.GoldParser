using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// A reduction token, which contains the child tokens reduced with the <see cref="ParentRule"/>.
	/// </summary>
	public sealed class Reduction: Token {
		private readonly Rule parentRule;
		private readonly Token[] tokens;

		internal Reduction(Rule parentRule, Token[] tokens): base() {
			if (parentRule == null) {
				throw new ArgumentNullException("parentRule");
			}
			if (tokens == null) {
				throw new ArgumentNullException("tokens");
			}
			this.parentRule = parentRule;
			this.tokens = tokens;
		}

		public override LineInfo Position {
			[DebuggerStepThrough]
			get {
				return tokens.Length > 0 ? tokens[0].Position : default(LineInfo);
			}
		}

		public override Token[] Children {
			[DebuggerStepThrough]
			get {
				return tokens;
			}
		}

		public override Symbol ParentSymbol {
			[DebuggerStepThrough]
			get {
				return parentRule.Head;
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach (Token token in tokens) {
				if (sb.Length > 0) {
					sb.Append(' ');
				}
				sb.Append(token);
			}
			return sb.ToString();
		}
	}
}