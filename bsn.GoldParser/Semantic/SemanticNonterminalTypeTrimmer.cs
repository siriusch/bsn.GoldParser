using System;
using System.Collections.ObjectModel;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticNonterminalTypeTrimmer<T>: SemanticNonterminalFactory<T> where T: SemanticToken {
#warning replace reflection with generated IL code
		private readonly int indexOfSymbolToKeep;
		private readonly ReadOnlyCollection<Type> inputTypes;

		public SemanticNonterminalTypeTrimmer(int indexOfSymbolToKeep) {
			this.indexOfSymbolToKeep = indexOfSymbolToKeep;
			inputTypes = Array.AsReadOnly(new[] {typeof(T)});
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return inputTypes;
			}
		}

		public override T Create(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			return (T)tokens[indexOfSymbolToKeep];
		}
	}
}
