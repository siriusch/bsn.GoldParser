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
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
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
			SymbolSet visited = new SymbolSet();
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

        protected override void InitializeInternal(ICollection<string> errors, bool trace, bool strongParameterCheck)
        {
			foreach (Type type in typeof(T).Assembly.GetTypes()) {
				if (typeof(T).IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract)) {
					SemanticTerminalFactory<T> terminalFactory = null;
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
							} catch (TargetInvocationException ex) {
								errors.Add(string.Format("Terminal {0} factory problem: {1}", symbol, ex.InnerException.Message));
							} catch (Exception ex) {
								errors.Add(string.Format("Terminal {0} factory problem: {1}", symbol, ex.Message));
							}
						}
					}
#pragma warning disable 612,618
					foreach (ConstructorInfo constructor in type.GetConstructors()) {
						foreach (RuleAttribute ruleAttribute in constructor.GetCustomAttributes(typeof(RuleAttribute), true)) {
							Rule rule = ruleAttribute.Bind(Grammar);
							if (rule == null) {
								errors.Add(string.Format("Rule {0} not found in grammar", ruleAttribute.Rule));
							} else {
								try {
									Type factoryType;
									ConstructorInfo factoryConstructor = null;
									if (ruleAttribute.IsGeneric) {
										factoryType = type.MakeGenericType(ruleAttribute.GenericTypeParameters);
										foreach (ConstructorInfo genericConstructor in factoryType.GetConstructors()) {
											foreach (RuleAttribute genericRuleAttribute in genericConstructor.GetCustomAttributes(typeof(RuleAttribute), true)) {
												if (ruleAttribute.Equals(genericRuleAttribute)) {
													factoryConstructor = genericConstructor;
													break;
												}
											}
											if (factoryConstructor != null) {
												break;
											}
										}
										Debug.Assert(factoryConstructor != null);
									} else {
										factoryType = type;
										factoryConstructor = constructor;
									}
									int[] parameterMapping;
									if (ruleAttribute.HasConstructorParameterMapping) {
										parameterMapping = ruleAttribute.ConstructorParameterMapping;
										if (parameterMapping == null) {
											parameterMapping = new int[ruleAttribute.AllowTruncationForConstructor ? constructor.GetParameters().Length : rule.SymbolCount];
											for (int i = 1; i < parameterMapping.Length; i++) {
												parameterMapping[i] = i;
											}
										}
									} else {
										parameterMapping = RuleDeclarationParser.BindConstructor(ruleAttribute.ParsedRule, constructor, ruleAttribute.AllowTruncationForConstructor);
									}
                                    if(ruleAttribute.StrictlyMatchCtorParameters.HasValue ? ruleAttribute.StrictlyMatchCtorParameters.Value : strongParameterCheck)
                                    {
                                        bool failed = false;
                                        foreach(var i in parameterMapping)
                                        {
                                            failed |= i < 0;
                                        }
                                        if (failed)
                                        {
                                            errors.Add(string.Format("Rule {0} does not map symbols to all the constructor arguments, but strict matching is set.",rule));
                                        }
                                    }

									SemanticNonterminalFactory<T> nonterminalFactory = CreateNonterminalFactory(factoryType, factoryConstructor, parameterMapping, rule.SymbolCount);
									RegisterNonterminalFactory(rule, nonterminalFactory);
								} catch (TargetInvocationException ex) {
									errors.Add(string.Format("Rule {0} factory problem: {1}", rule, ex.InnerException.Message));
								} catch (Exception ex) {
									errors.Add(string.Format("Rule {0} factory problem: {1}", rule, ex.Message));
								}
							}
						}
					}
#pragma warning restore 612,618
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

		private SemanticNonterminalFactory<T> CreateNonterminalFactory(Type type, ConstructorInfo constructor, int[] parameterMapping, int handleCount) {
			return (SemanticNonterminalFactory<T>)Activator.CreateInstance(typeof(SemanticNonterminalTypeFactory<,>).MakeGenericType(typeof(T), type), constructor, parameterMapping, handleCount, typeof(T));
		}

		private SemanticTerminalFactory<T> CreateTerminalFactory(Type type) {
			return (SemanticTerminalFactory<T>)Activator.CreateInstance(typeof(SemanticTerminalTypeFactory<,>).MakeGenericType(typeof(T), type));
		}

		private void MemorizeType(SemanticTokenFactory<T> factory, Symbol symbol) {
			if (factory.IsStaticOutputType) {
				symbolTypeMap.SetTypeForSymbol(symbol, factory.OutputType);
			}
		}
	}
}