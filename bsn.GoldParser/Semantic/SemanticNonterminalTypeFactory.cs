using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticNonterminalTypeFactory<T>: SemanticNonterminalFactory<T> where T: SemanticToken {
		private readonly SemanticNonterminalTypeFactoryHelper.Activator<T> activator;
		private readonly ReadOnlyCollection<Type> inputTypes;

		public SemanticNonterminalTypeFactory(ConstructorInfo constructor) {
			if (constructor == null) {
				throw new ArgumentNullException("constructor");
			}
			inputTypes = Array.AsReadOnly(Array.ConvertAll(constructor.GetParameters(), input => input.ParameterType));
			activator = SemanticNonterminalTypeFactoryHelper.CreateActivator(this, constructor);
			Debug.Assert(activator != null);
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return inputTypes;
			}
		}

		public override T Create(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
			Debug.Assert((tokens != null) && (tokens.Count == inputTypes.Count));
			return activator(tokens);
		}
	}
}
