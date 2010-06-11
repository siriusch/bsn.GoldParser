using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
		private class ExplicitTrimFactory: SemanticNonterminalFactory {
			private readonly int handleIndex;
			private readonly Symbol handleSymbol;
			private readonly SemanticTypeActions<T> owner;
			private int cacheVersion = int.MinValue;
			private Type type;

			public ExplicitTrimFactory(SemanticTypeActions<T> owner, Rule rule, int handleIndex) {
				Debug.Assert(owner != null);
				Debug.Assert(rule != null);
				this.owner = owner;
				this.handleIndex = handleIndex;
				handleSymbol = rule[handleIndex];
			}

			public override ReadOnlyCollection<Type> InputTypes {
				get {
					UpdateCache();
					return Array.AsReadOnly(new[] {type});
				}
			}

			public override Type OutputType {
				get {
					UpdateCache();
					return type;
				}
			}

			internal override SemanticToken CreateInternal(Rule rule, ReadOnlyCollection<SemanticToken> tokens) {
				SemanticToken result = tokens[handleIndex];
				Debug.Assert(((IToken)result).Symbol == handleSymbol);
				Debug.Assert(OutputType.IsAssignableFrom(result.GetType()));
				return result;
			}

			private void UpdateCache() {
				if (cacheVersion != owner.symbolTypeMap.Version) {
					type = owner.GetOutputTypeOfSymbol(handleSymbol);
					cacheVersion = owner.symbolTypeMap.Version;
				}
			}
		}

		private class ResolvedTypes: Dictionary<Symbol, Type> {
			public static void Acquire(ref ResolvedTypes resolvedTypes) {
				if (resolvedTypes == null) {
					resolvedTypes = new ResolvedTypes();
				} else {
					resolvedTypes.usageCount++;
				}
			}

			public static void Release(ref ResolvedTypes resolvedTypes) {
				if (resolvedTypes == null) {
					throw new ArgumentNullException("resolvedTypes");
				}
				resolvedTypes.usageCount--;
				Debug.Assert(resolvedTypes.usageCount >= 0);
				if (resolvedTypes.usageCount == 0) {
					resolvedTypes = null;
				}
			}

			private int usageCount;

			private ResolvedTypes() {
				usageCount = 1;
			}
		}

		[ThreadStatic]
		private static ResolvedTypes resolvedTypes;

		private readonly SymbolTypeMap<T> symbolTypeMap = new SymbolTypeMap<T>();
		private readonly object sync = new object();
		private bool initialized;

		public SemanticTypeActions(CompiledGrammar grammar): base(grammar) {
			// then we go through all types which are candidates for carrying rule or terminal attributes and register those
		}

		public Type GetOutputTypeOfSymbol(Symbol symbol) {
			Type result;
			if (TryGetOutputTypeOfSymbol(symbol, out result)) {
				return result;
			}
			return typeof(T);
		}

		public bool TryGetOutputTypeOfSymbol(Symbol symbol, out Type result) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			ResolvedTypes.Acquire(ref resolvedTypes);
			try {
				if (!resolvedTypes.TryGetValue(symbol, out result)) {
					resolvedTypes.Add(symbol, null);
					if (symbol.Kind == SymbolKind.Nonterminal) {
						foreach (Rule rule in Grammar.GetRulesForSymbol(symbol)) {
							Type ruleSymbolType = null;
							SemanticNonterminalFactory factory;
							if (TryGetNonterminalFactory(rule, out factory)) {
								ruleSymbolType = factory.OutputType;
							} else if (rule.ContainsOneNonterminal) {
								TryGetOutputTypeOfSymbol(rule.RuleSymbol, out ruleSymbolType);
							}
							if (result == null) {
								result = ruleSymbolType;
							} else if (ruleSymbolType != null) {
								result = symbolTypeMap.GetCommonBaseType(result, ruleSymbolType);
							}
						}
					} else {
						SemanticTerminalFactory factory;
						if (TryGetTerminalFactory(symbol, out factory)) {
							result = factory.OutputType;
						}
					}
					resolvedTypes[symbol] = result;
				}
				return result != null;
			} finally {
				ResolvedTypes.Release(ref resolvedTypes);
			}
		}

		protected void Initialize() {
			lock (sync) {
				if (!initialized) {
					InitializeInternal();
					initialized = true;
				}
			}
		}

		protected virtual void InitializeInternal() {
			foreach (Type type in typeof(T).Assembly.GetTypes()) {
				if (typeof(T).IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract) && (!type.IsGenericTypeDefinition)) {
					foreach (TerminalAttribute terminalAttribute in type.GetCustomAttributes(typeof(TerminalAttribute), true)) {
						Symbol symbol = terminalAttribute.Bind(Grammar);
						if (symbol == null) {
							throw new InvalidOperationException(string.Format("Terminal {0} not found in grammar", terminalAttribute.SymbolName));
						}
						RegisterTerminalFactory(symbol, CreateTerminalFactory(type));
					}
					foreach (ConstructorInfo constructor in type.GetConstructors()) {
						foreach (RuleAttribute ruleAttribute in type.GetCustomAttributes(typeof(RuleAttribute), true)) {
							Rule rule = ruleAttribute.Bind(Grammar);
							if (rule == null) {
								throw new InvalidOperationException(string.Format("Nonterminal {0} not found in grammar", ruleAttribute.Rule));
							}
							RegisterNonterminalFactory(rule, CreateNonterminalFactory(type, constructor));
						}
					}
				}
			}
			// finally we look for all trim rules in the assembly
			foreach (RuleTrimAttribute ruleTrimAttribute in typeof(T).Assembly.GetCustomAttributes(typeof(RuleTrimAttribute), false)) {
				Rule rule = ruleTrimAttribute.Bind(Grammar);
				if (rule == null) {
					throw new InvalidOperationException(string.Format("Nonterminal {0} not found in grammar", ruleTrimAttribute.Rule));
				}
				RegisterNonterminalFactory(rule, new ExplicitTrimFactory(this, rule, ruleTrimAttribute.TrimSymbolIndex));
			}
		}

		protected override void RegisterNonterminalFactory(Rule rule, SemanticNonterminalFactory factory) {
			base.RegisterNonterminalFactory(rule, factory);
			MemorizeType(factory, rule.RuleSymbol);
		}

		protected override void RegisterTerminalFactory(Symbol symbol, SemanticTerminalFactory factory) {
			base.RegisterTerminalFactory(symbol, factory);
			MemorizeType(factory, symbol);
		}

		private SemanticNonterminalFactory CreateNonterminalFactory(Type type, ConstructorInfo constructor) {
#warning maybe replace activator with generated IL code
			return (SemanticNonterminalFactory)Activator.CreateInstance(typeof(SemanticNonterminalTypeFactory<>).MakeGenericType(type), constructor);
		}

		private SemanticTerminalFactory CreateTerminalFactory(Type type) {
#warning maybe replace activator with generated IL code
			return (SemanticTerminalFactory)Activator.CreateInstance(typeof(SemanticTerminalTypeFactory<>).MakeGenericType(type));
		}

		private void MemorizeType(SemanticTokenFactory factory, Symbol symbol) {
			if (factory.IsStaticOutputType) {
				symbolTypeMap.MemorizeTypeForSymbol(symbol, factory.OutputType);
			}
		}
	}
}