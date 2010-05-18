using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticProcessor<T>: LalrProcessor<T> where T: IToken {
		private readonly SemanticActions<T> actions;

		private static CompiledGrammar GetGrammar(SemanticActions<T> actions) {
			if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			return actions.Grammar;
		}

		public SemanticProcessor(ITokenizer<T> tokenizer, SemanticActions<T> actions)
			: base(tokenizer) {
			this.actions = actions;
		}

		protected override bool CanTrim(Rule rule) {
			SemanticTokenFactory<T> dummy;
			return !actions.TryGetFactory(rule, out dummy);
		}

		protected override T CreateReduction(Rule rule, ReadOnlyCollection<T> children) {
			SemanticTokenFactory<T> factory;
			if (actions.TryGetFactory(rule, out factory)) {
				Debug.Assert(factory != null);
				return factory.CreateInternal(rule, children);
			}
			throw new InvalidOperationException(string.Format("Missing a token type for the rule {0}", rule.Definition));
		}
	}
}
