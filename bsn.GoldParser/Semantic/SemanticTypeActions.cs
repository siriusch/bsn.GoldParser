// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Ars�ne von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-goldparser.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
		private delegate void GetFactoryType<TU, in TA>(TU methodBase, TA attribute, Type type, out Type factoryType, out TU factoryConstructor) where TU: MethodBase where TA: Attribute, IGrammarBindableAttribute;

		private static SemanticNonterminalFactory<T> CreateNonterminalFactory(Type type, MethodBase methodBase, int[] parameterMapping, int handleCount) {
			return (SemanticNonterminalFactory<T>)Activator.CreateInstance(typeof(SemanticNonterminalTypeFactory<,>).MakeGenericType(typeof(T), type), methodBase, parameterMapping, handleCount, typeof(T));
		}

		private static SemanticTerminalFactory<T> CreateTerminalFactory(Type type, MethodBase methodBase, object[] additionalArguments) {
			return (SemanticTerminalFactory<T>)Activator.CreateInstance(typeof(SemanticTerminalTypeFactory<,>).MakeGenericType(typeof(T), type), methodBase, additionalArguments);
		}

		private static void GetConstructorFactoryType(ConstructorInfo constructor, IGrammarBindableAttribute attribute, Type type, out Type factoryType, out ConstructorInfo factoryConstructor) {
			if (attribute.IsGeneric) {
				factoryType = type.MakeGenericType(attribute.GenericTypeParameters);
				if (constructor != null) {
					foreach (ConstructorInfo genericConstructor in factoryType.GetConstructors()) {
						foreach (Attribute genericRuleAttribute in genericConstructor.GetCustomAttributes(attribute.GetType(), true)) {
							if (attribute.Equals(genericRuleAttribute)) {
								factoryConstructor = genericConstructor;
								return;
							}
						}
					}
					throw new InvalidOperationException("The matching generic constructor was not found");
				}
			} else {
				factoryType = type;
			}
			factoryConstructor = constructor;
		}

		private static void GetMethodFactoryType(MethodInfo methodInfo, IGrammarBindableAttribute attribute, Type type, out Type factoryType, out MethodInfo factoryMethod) {
			if (attribute.IsGeneric) {
				factoryMethod = methodInfo.MakeGenericMethod(attribute.GenericTypeParameters);
				factoryType = factoryMethod.ReturnType;
			} else {
				factoryType = methodInfo.ReturnType;
				factoryMethod = methodInfo;
			}
		}

		private static int[] ParameterMapping(bool strongParameterCheck, Rule rule, RuleAttribute ruleAttribute, MethodBase methodBase) {
#pragma warning disable 612
			int[] parameterMapping;
			if (ruleAttribute.HasConstructorParameterMapping) {
				parameterMapping = ruleAttribute.ConstructorParameterMapping;
				if (parameterMapping == null) {
					parameterMapping = new int[ruleAttribute.AllowTruncationForConstructor ? methodBase.GetParameters().Length : rule.SymbolCount];
					for (int i = 1; i < parameterMapping.Length; i++) {
						parameterMapping[i] = i;
					}
				}
			} else {
				parameterMapping = RuleDeclarationParser.BindMethodBase(ruleAttribute.ParsedRule, methodBase, ruleAttribute.AllowTruncationForConstructor, ruleAttribute.StrictlyMatchParametersOrDefault(strongParameterCheck));
			}
			return parameterMapping;
#pragma warning restore 612
		}

		private readonly SymbolTypeMap<T> symbolTypeMap = new SymbolTypeMap<T>();

		public SemanticTypeActions(CompiledGrammar grammar): base(grammar) {}

		public override Type GetSymbolOutputType(Symbol symbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != Grammar) {
				throw new ArgumentException("The given symbol belongs to another grammar", "symbol");
			}
			Queue<Symbol> pending = new Queue<Symbol>();
			pending.Enqueue(symbol);
			GrammarObjectSet<Symbol> visited = new GrammarObjectSet<Symbol>();
			Type bestMatch = null;
			while (pending.Count > 0) {
				symbol = pending.Dequeue();
				if (visited.Set(symbol)) {
					foreach (SemanticTokenFactory<T> factory in GetTokenFactoriesForSymbol(symbol)) {
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

		protected override void InitializeInternal(ICollection<string> errors, bool trace, bool strongParameterCheck) {
			foreach (Type type in typeof(T).Assembly.GetTypes()) {
				if (typeof(T).IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract)) {
					foreach (TerminalAttribute terminalAttribute in type.GetCustomAttributes(typeof(TerminalAttribute), true)) {
						ProcessTerminalAttribute<ConstructorInfo>(errors, terminalAttribute, type, null, GetConstructorFactoryType);
					}
					foreach (ConstructorInfo constructor in type.GetConstructors()) {
						foreach (RuleAttribute ruleAttribute in constructor.GetCustomAttributes(typeof(RuleAttribute), true)) {
							ProcessRuleAttribute(errors, strongParameterCheck, ruleAttribute, type, constructor, GetConstructorFactoryType);
						}
						foreach (TerminalAttribute terminalAttribute in constructor.GetCustomAttributes(typeof(TerminalAttribute), true)) {
							ProcessTerminalAttribute(errors, terminalAttribute, type, constructor, GetConstructorFactoryType);
						}
					}
				}
				//examine the static methods
				foreach (MethodInfo methodInfo in type.GetMethods()) {
					if (typeof(T).IsAssignableFrom(methodInfo.ReturnType)) {
						foreach (TerminalAttribute terminalAttribute in methodInfo.GetCustomAttributes(typeof(TerminalAttribute), true)) {
							if (methodInfo.IsStatic) {
								ProcessTerminalAttribute(errors, terminalAttribute, type, methodInfo, GetMethodFactoryType);
							} else {
								errors.Add(string.Format("Terminal {0} is assigned to a non-static method, which is not allowed.", terminalAttribute.SymbolName));
							}
						}
						foreach (RuleAttribute ruleAttribute in methodInfo.GetCustomAttributes(typeof(RuleAttribute), true)) {
							if (methodInfo.IsStatic) {
								ProcessRuleAttribute(errors, strongParameterCheck, ruleAttribute, type, methodInfo, GetMethodFactoryType);
							} else {
								errors.Add(string.Format("Rule {0} is assigned to a non-static method, which is not allowed.", ruleAttribute.Rule));
							}
						}
					}
				}
			}
			// finally we look for all trim rules in the assembly
			foreach (RuleTrimAttribute ruleTrimAttribute in typeof(T).Assembly.GetCustomAttributes(typeof(RuleTrimAttribute), false)) {
				if ((ruleTrimAttribute.SemanticTokenType == null) || typeof(T).Equals(ruleTrimAttribute.SemanticTokenType)) {
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
		}

		protected override void RegisterNonterminalFactory(Rule rule, SemanticNonterminalFactory<T> factory) {
			base.RegisterNonterminalFactory(rule, factory);
			MemorizeType(factory, rule.RuleSymbol);
		}

		protected override void RegisterTerminalFactory(Symbol symbol, SemanticTerminalFactory<T> factory) {
			base.RegisterTerminalFactory(symbol, factory);
			MemorizeType(factory, symbol);
		}

		private void MemorizeType(SemanticTokenFactory<T> factory, Symbol symbol) {
			if (factory.IsStaticOutputType) {
				symbolTypeMap.SetTypeForSymbol(symbol, factory.OutputType);
			}
		}

		private void ProcessRuleAttribute<TU>(ICollection<string> errors, bool strongParameterCheck, RuleAttribute ruleAttribute, Type type, TU methodBase, GetFactoryType<TU, RuleAttribute> getFactoryType) where TU: MethodBase {
			Rule rule = ruleAttribute.Bind(Grammar);
			if (rule == null) {
				errors.Add(string.Format("Rule {0} not found in grammar", ruleAttribute.Rule));
			} else {
				try {
					Type factoryType;
					TU factoryConstructor;
					getFactoryType(methodBase, ruleAttribute, type, out factoryType, out factoryConstructor);
					int[] parameterMapping = ParameterMapping(strongParameterCheck, rule, ruleAttribute, methodBase);
					SemanticNonterminalFactory<T> nonterminalFactory = CreateNonterminalFactory(factoryType, factoryConstructor, parameterMapping, rule.SymbolCount);
					RegisterNonterminalFactory(rule, nonterminalFactory);
				} catch (TargetInvocationException ex) {
					errors.Add(string.Format("Rule {0} factory problem: {1}", rule, ex.InnerException.Message));
				} catch (Exception ex) {
					errors.Add(string.Format("Rule {0} factory problem: {1}", rule, ex.Message));
				}
			}
		}

		private void ProcessTerminalAttribute<TU>(ICollection<string> errors, TerminalAttribute terminalAttribute, Type type, TU methodBase, GetFactoryType<TU, TerminalAttribute> getFactoryType) where TU: MethodBase {
			Symbol symbol = terminalAttribute.Bind(Grammar);
			if (symbol == null) {
				errors.Add(string.Format("Terminal {0} not found in grammar", terminalAttribute.SymbolName));
			} else {
				try {
					Type factoryType;
					TU factoryConstructor;
					getFactoryType(methodBase, terminalAttribute, type, out factoryType, out factoryConstructor);
					SemanticTerminalFactory<T> terminalFactory = CreateTerminalFactory(factoryType, methodBase, terminalAttribute.Arguments);
					RegisterTerminalFactory(symbol, terminalFactory);
				} catch (TargetInvocationException ex) {
					errors.Add(string.Format("Terminal {0} factory problem: {1}", symbol, ex.InnerException.Message));
				} catch (Exception ex) {
					errors.Add(string.Format("Terminal {0} factory problem: {1}", symbol, ex.Message));
				}
			}
		}
	}
}
