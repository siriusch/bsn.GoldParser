using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticProcessor<T>: LalrProcessor<SemanticToken> where T: SemanticToken {
		private readonly SemanticActions<T> actions;

		public SemanticProcessor(TextReader reader, SemanticActions<T> actions): this(actions.CreateTokenizer(reader), actions) {}

		public SemanticProcessor(ITokenizer<SemanticToken> tokenizer, SemanticActions<T> actions): base(tokenizer) {
			if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			if (tokenizer.Grammar != actions.Grammar) {
				throw new ArgumentException("Mismatch of tokenizer and action grammars");
			}
			this.actions = actions;
		}

		protected override bool CanTrim(Rule rule) {
			SemanticNonterminalFactory dummy;
			return !actions.TryGetNonterminalFactory(rule, out dummy);
		}

		protected override SemanticToken CreateReduction(Rule rule, ReadOnlyCollection<SemanticToken> children) {
			SemanticNonterminalFactory factory;
			if (actions.TryGetNonterminalFactory(rule, out factory)) {
				Debug.Assert(factory != null);
				SemanticToken result = factory.CreateInternal(rule, children);
				result.Initialize(rule.RuleSymbol, (children.Count > 0) ? ((IToken)children[0]).Position : default(LineInfo));
				return result;
			}
			throw new InvalidOperationException(string.Format("Missing a token type for the rule {0}", rule.Definition));
		}
	}
}
