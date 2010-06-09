using System;
using System.Collections.ObjectModel;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticNonterminalTypeFactory<T>: SemanticNonterminalFactory<T> where T: SemanticToken {
#warning replace reflection with generated IL code
		private readonly ConstructorInfo constructor;
		private readonly ReadOnlyCollection<Type> inputTypes;

		public SemanticNonterminalTypeFactory(ConstructorInfo constructor) {
			if (constructor == null) {
				throw new ArgumentNullException("constructor");
			}
			this.constructor = constructor;
			inputTypes = Array.AsReadOnly(Array.ConvertAll(constructor.GetParameters(), input => input.ParameterType));
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return inputTypes;
			}
		}

		public override T Create(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			SemanticToken[] args = new SemanticToken[tokens.Count];
			tokens.CopyTo(args, 0);
			return (T)constructor.Invoke(args);
		}
	}
}