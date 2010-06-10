using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	public class SemanticTypeActions<T>: SemanticActions<T> where T: SemanticToken {
		public SemanticTypeActions(CompiledGrammar grammar)
			: base(grammar) {
			// first, wen find all possible implicit trims, so that we can keep track of those used otherwise
			Dictionary<Rule, Symbol> implicitTrim = new Dictionary<Rule, Symbol>();
			for (int i = 0; i < Grammar.RuleCount; i++) {
				Rule rule = Grammar.GetRule(i);
				if (rule.ContainsOneNonterminal) {
					implicitTrim.Add(rule, rule[0]);
				}
			}
			// then we go through all types which are candidates for carrying rule or terminal attributes and register those
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
			// now we start building a dependency map, which is later used to process the types of the trim reductions
			SymbolDependencyMap dependencies = new SymbolDependencyMap();
			// next we look for all trim rules in the assembly, and again remove implicit trims if they are defined explicitly
			List<KeyValuePair<RuleTrimAttribute, Rule>> trimRules = new List<KeyValuePair<RuleTrimAttribute, Rule>>();
			foreach (RuleTrimAttribute ruleTrimAttribute in typeof(T).Assembly.GetCustomAttributes(typeof(RuleTrimAttribute), false)) {
				Rule rule = ruleTrimAttribute.Bind(Grammar);
				if (rule == null) {
					throw new InvalidOperationException(string.Format("Nonterminal {0} not found in grammar", ruleTrimAttribute.Rule));
				}
				dependencies.AddDependecy(rule.RuleSymbol, rule[ruleTrimAttribute.TrimSymbolIndex]);
				trimRules.Add(new KeyValuePair<RuleTrimAttribute, Rule>(ruleTrimAttribute, rule));
				implicitTrim.Remove(rule);
			}
			foreach (KeyValuePair<Rule, Symbol> pair in implicitTrim) {
				dependencies.AddDependecy(pair.Key.RuleSymbol, pair.Value);
			}
			// the dependencies are now completely resolved, and we can start 
			foreach (KeyValuePair<RuleTrimAttribute, Rule> pair in trimRules) {
#warning Missing: item order must be so that dependencies are correctly handled for type resolution, and also respect the implicit trimming rules
				int indexOfSymbolToKeep = pair.Key.TrimSymbolIndex;
				Symbol symbolToKeep = pair.Value[indexOfSymbolToKeep];
				RegisterNonterminalFactory(pair.Value, CreateTrimFactory(typeUtility.GetSymbolType(symbolToKeep), indexOfSymbolToKeep));
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
