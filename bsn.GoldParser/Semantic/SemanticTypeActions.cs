using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
		public SemanticTypeActions(CompiledGrammar grammar)
			: base(grammar) {
#warning Add tracking of possible implicit trims, and remove them when an explicit entry is specified
			Dictionary<Rule, Symbol> implicitTrim = new Dictionary<Rule, Symbol>();
			for (int i = 0; i < Grammar.RuleCount; i++) {
				Rule rule = Grammar.GetRule(i);
				if (rule.ContainsOneNonterminal) {
					implicitTrim.Add(rule, rule[0]);
				}
			}
			TypeUtility<T> typeUtility = new TypeUtility<T>();
			foreach (Type type in typeof(T).Assembly.GetTypes()) {
				if (typeof(T).IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract) && (!type.IsGenericTypeDefinition)) {
					foreach (TerminalAttribute terminalAttribute in type.GetCustomAttributes(typeof(TerminalAttribute), true)) {
						Symbol symbol = terminalAttribute.Bind(Grammar);
						if (symbol == null) {
							throw new InvalidOperationException(string.Format("Terminal {0} not found in grammar", terminalAttribute.SymbolName));
						}
						RegisterTerminalFactory(symbol, CreateTerminalFactory(type));
						typeUtility.MemorizeTypeForSymbol(symbol, type);
					}
					foreach (ConstructorInfo constructor in type.GetConstructors()) {
						foreach (RuleAttribute ruleAttribute in type.GetCustomAttributes(typeof(RuleAttribute), true)) {
							Rule rule = ruleAttribute.Bind(Grammar);
							if (rule == null) {
								throw new InvalidOperationException(string.Format("Nonterminal {0} not found in grammar", ruleAttribute.Rule));
							}
							implicitTrim.Remove(rule);
							RegisterNonterminalFactory(rule, CreateNonterminalFactory(type, constructor));
							typeUtility.MemorizeTypeForSymbol(rule.RuleSymbol, type);
						}
					}
				}
			}
			RuleTrimAttribute[] ruleTrimAttributes = (RuleTrimAttribute[])typeof(T).Assembly.GetCustomAttributes(typeof(RuleTrimAttribute), false);
#warning Missing: item order must be so that dependencies are correctly handled for type resolution, and also respect the implicit trimming rules
			foreach (RuleTrimAttribute ruleTrimAttribute in ruleTrimAttributes) {
				Rule rule = ruleTrimAttribute.Bind(Grammar);
				if (rule == null) {
					throw new InvalidOperationException(string.Format("Nonterminal {0} not found in grammar", ruleTrimAttribute.Rule));
				}
				implicitTrim.Remove(rule);
				Symbol symbolToKeep = rule[ruleTrimAttribute.IndexOfSymbolToKeep];
				RegisterNonterminalFactory(rule, CreateTrimFactory(typeUtility.GetSymbolType(symbolToKeep), ruleTrimAttribute.IndexOfSymbolToKeep));
			}
		}

		private SemanticNonterminalFactory CreateTrimFactory(Type type, int indexOfSymbolToKeep) {
#warning maybe replace activator with generated IL code
			return (SemanticNonterminalFactory)Activator.CreateInstance(typeof(SemanticNonterminalTypeTrimmer<>).MakeGenericType(type), indexOfSymbolToKeep);
		}

		private SemanticNonterminalFactory CreateNonterminalFactory(Type type, ConstructorInfo constructor) {
#warning maybe replace activator with generated IL code
			return (SemanticNonterminalFactory)Activator.CreateInstance(typeof(SemanticNonterminalTypeFactory<>).MakeGenericType(type), constructor);
		}

		private SemanticTerminalFactory CreateTerminalFactory(Type type) {
#warning maybe replace activator with generated IL code
			return (SemanticTerminalFactory)Activator.CreateInstance(typeof(SemanticTerminalTypeFactory<>).MakeGenericType(type));
		}
	}
}
