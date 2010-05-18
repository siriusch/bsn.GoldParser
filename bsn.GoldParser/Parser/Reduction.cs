// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// A reduction token, which contains the child tokens reduced with the <see cref="ParentRule"/>.
	/// </summary>
	public class Reduction: Token {
		private readonly Rule rule;
		private readonly ReadOnlyCollection<Token> tokens;

		internal Reduction(Rule rule, ReadOnlyCollection<Token> tokens): base() {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if (tokens == null) {
				throw new ArgumentNullException("tokens");
			}
			this.rule = rule;
			this.tokens = tokens;
		}

		public ReadOnlyCollection<Token> Children {
			[DebuggerStepThrough]
			get {
				return tokens;
			}
		}

		public sealed override Symbol Symbol {
			[DebuggerStepThrough]
			get {
				return rule.RuleSymbol;
			}
		}

		public sealed override LineInfo Position {
			[DebuggerStepThrough]
			get {
				return tokens.Count > 0 ? tokens[0].Position : default(LineInfo);
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