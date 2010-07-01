using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticNonterminalTypeFactory<T>: SemanticNonterminalFactory<T> where T: SemanticToken {
		private readonly SemanticNonterminalTypeFactoryHelper.Activator<T> activator;
		private readonly ReadOnlyCollection<Type> inputTypes;

		public SemanticNonterminalTypeFactory(ConstructorInfo constructor, int[] parameterMapping, int handleCount, Type baseTokenType) {
			if (constructor == null) {
				throw new ArgumentNullException("constructor");
			}
			if (parameterMapping == null) {
				throw new ArgumentNullException("parameterMapping");
			}
			if (baseTokenType == null) {
				throw new ArgumentNullException("baseTokenType");
			}
			ParameterInfo[] parameters = constructor.GetParameters();
			if (handleCount < parameters.Length) {
				throw new ArgumentOutOfRangeException("handleCount");
			}
			if (parameterMapping.Length != parameters.Length) {
				throw new ArgumentException("The parameter mapping must have exactly as many items as the constructor has parameters", "parameterMapping");
			}
			Type[] inputTypeBuilder = new Type[handleCount];
			for (int i = 0; i < handleCount; i++) {
				inputTypeBuilder[i] = baseTokenType;
			}
			foreach (ParameterInfo parameter in parameters) {
				inputTypeBuilder[parameterMapping[parameter.Position]] = parameter.ParameterType;
			}
			inputTypes = Array.AsReadOnly(inputTypeBuilder);
			activator = SemanticNonterminalTypeFactoryHelper.CreateActivator(this, constructor, parameterMapping);
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