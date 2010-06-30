using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
		private readonly SymbolTypeMap<T> symbolTypeMap = new SymbolTypeMap<T>();

		public SemanticTypeActions(CompiledGrammar grammar): base(grammar) {
			// then we go through all types which are candidates for carrying rule or terminal attributes and register those
		}

		public override Type GetSymbolOutputType(Symbol symbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != Grammar) {
				throw new ArgumentException("The given symbol belongs to another grammar", "symbol");
			}
			Queue<Symbol> pending = new Queue<Symbol>();
			pending.Enqueue(symbol);
			SymbolSet visited = new SymbolSet();
			Type bestMatch = null;
			while (pending.Count > 0) {
				symbol = pending.Dequeue();
				if (visited.Set(symbol)) {
					foreach (SemanticTokenFactory factory in GetTokenFactoriesForSymbol(symbol)) {
						Symbol redirectForOutputType = factory.RedirectForOutputType;
						if (redirectForOutputType == null) {
							symbolTypeMap.ApplyCommonBaseType(ref bestMatch, factory.OutputType);
						} else {
							pending.Enqueue(redirectForOutputType);
						}
					}
				}
			}
			return bestMatch ?? typeof(T);
		}

		protected override void InitializeInternal(ICollection<string> errors) {
			foreach (Type type in typeof(T).Assembly.GetTypes()) {
				if (typeof(T).IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract)) {
					SemanticTerminalFactory terminalFactory = null;
					foreach (TerminalAttribute terminalAttribute in type.GetCustomAttributes(typeof(TerminalAttribute), true)) {
						Symbol symbol = terminalAttribute.Bind(Grammar);
						if (symbol == null) {
							errors.Add(string.Format("Terminal {0} not found in grammar", terminalAttribute.SymbolName));
						} else {
							try {
								Type factoryType = (terminalAttribute.IsGeneric) ? type.MakeGenericType(terminalAttribute.GenericTypes) : type;
								if (terminalFactory == null) {
									terminalFactory = CreateTerminalFactory(factoryType);
								}
								RegisterTerminalFactory(symbol, terminalFactory);
								if (factoryType != type) {
									terminalFactory = null; // don't keep generic factories
								}
							} catch (InvalidOperationException ex) {
								errors.Add(string.Format("Terminal {0} factory problem: {1}", symbol, ex.Message));
							}
						}
					}
					foreach (ConstructorInfo constructor in type.GetConstructors()) {
						SemanticNonterminalFactory nonterminalFactory = null;
						foreach (RuleAttribute ruleAttribute in constructor.GetCustomAttributes(typeof(RuleAttribute), true)) {
							Rule rule = ruleAttribute.Bind(Grammar);
							if (rule == null) {
								errors.Add(string.Format("Rule {0} not found in grammar", ruleAttribute.Rule));
							} else {
								try {
									Type factoryType;
									ConstructorInfo factoryConstructor = null;
									if (ruleAttribute.IsGeneric) {
										factoryType = type.MakeGenericType(ruleAttribute.GenericTypes);
										foreach (ConstructorInfo genericConstructor in factoryType.GetConstructors()) {
											foreach (RuleAttribute genericRuleAttribute in genericConstructor.GetCustomAttributes(typeof(RuleAttribute), true)) {
												if (ruleAttribute.Equals(genericRuleAttribute)) {
													factoryConstructor = genericConstructor;
													break;
												}
											}
											Debug.Assert(factoryConstructor != null);
										}
									} else {
										factoryType = type;
										factoryConstructor = constructor;
									}
									if (nonterminalFactory == null) {
										nonterminalFactory = CreateNonterminalFactory(factoryType, factoryConstructor);
									}
									RegisterNonterminalFactory(rule, nonterminalFactory);
									if (factoryType != type) {
										nonterminalFactory = null; // don't keep generic factories
									}
								}
								catch (InvalidOperationException ex) {
									errors.Add(string.Format("Rule {0} factory problem: {1}", rule, ex.Message));
								}
							}
						}
					}
				}
			}
			// finally we look for all trim rules in the assembly
			foreach (RuleTrimAttribute ruleTrimAttribute in typeof(T).Assembly.GetCustomAttributes(typeof(RuleTrimAttribute), false)) {
				Rule rule = ruleTrimAttribute.Bind(Grammar);
				if (rule == null) {
					errors.Add(string.Format("Rule {0} not found in grammar", ruleTrimAttribute.Rule));
				} else {
					try {
						RegisterNonterminalFactory(rule, new SemanticTrimFactory<T>(this, rule, ruleTrimAttribute.TrimSymbolIndex));
					} catch (InvalidOperationException ex) {
						errors.Add(string.Format("Trim tule {0} factory problem: {1}", rule, ex.Message));
					}
				}
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
				symbolTypeMap.SetTypeForSymbol(symbol, factory.OutputType);
			}
		}
	}
}
