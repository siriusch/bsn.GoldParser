using System;
using System.Diagnostics;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticProcessor<T>: LalrProcessor where T: Token {
		private readonly SemanticActions<T> actions;

		private static CompiledGrammar GetGrammar(SemanticActions<T> actions) {
			if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			return actions.Grammar;
		}

		public SemanticProcessor(ITokenizer tokenizer, SemanticActions<T> actions)
			: base(tokenizer) {
			this.actions = actions;
		}

		protected override bool CanTrim(Rule rule) {
			SemanticTokenFactory dummy;
			return !actions.TryGetFactory(rule, out dummy);
		}

		protected override Token CreateReduction(Rule rule, IToken[] children) {
			SemanticTokenFactory factory;
			if (actions.TryGetFactory(rule, out factory)) {
				Debug.Assert(factory != null);
				return factory.CreateInternal(rule, children);
			}
			return base.CreateReduction(rule, children);
		}

		protected override Token ConvertToken(TextToken inputToken) {
			SemanticTokenConverter converter;
			if (actions.TryGetConverter(inputToken.Symbol, out converter)) {
				Debug.Assert(converter != null);
				return converter.ConvertInternal(inputToken);
			}
			return base.ConvertToken(inputToken);
		}

		public T Result {
			get {
				return CurrentToken as T;
			}
		}
	}
}
