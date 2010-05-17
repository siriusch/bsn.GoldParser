using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public class RuleDeclarationParser {
		private static readonly CompiledGrammar ruleGrammar = CompiledGrammar.Load(typeof(RuleDeclarationParser), "GoldRuleDeclaration.cgt");

		internal static CompiledGrammar RuleGrammar {
			get {
				return ruleGrammar;
			}
		}

		internal static bool TryParse(string ruleString, out Reduction ruleToken) {
			using (StringReader reader = new StringReader(ruleString)) {
				ITokenizer tokenizer = new Tokenizer(reader, ruleGrammar);
				LalrProcessor processor = new LalrProcessor(tokenizer);
				ParseMessage message;
				do {
					message = processor.Parse();
					if (message == ParseMessage.Accept) {
						ruleToken = (Reduction)processor.CurrentToken;
						return true;
					}
				} while (CompiledGrammar.CanContinueParsing(message));
			}
			ruleToken = null;
			return false;
		}

		internal static bool TryBind(Reduction ruleDeclaration, CompiledGrammar grammar, out Rule rule) {
			Symbol ruleSymbol;
			if (grammar.TryGetSymbol(ruleDeclaration.Children[0].Text, out ruleSymbol)) {
				ReadOnlyCollection<Rule> rules;
				if (grammar.TryGetRulesForSymbol(ruleSymbol, out rules)) {
					List<Symbol> symbols = new List<Symbol>();
					Reduction handle = (Reduction)ruleDeclaration.Children[2];
					while (handle.Children.Length == 2) {
						Symbol symbol;
						if (!grammar.TryGetSymbol(handle.Children[0].ToString(), out symbol)) {
							symbols = null;
							break;
						}
						symbols.Add(symbol);
						handle = (Reduction)handle.Children[1];
					}
					if (symbols != null) {
						foreach (Rule currentRule in rules) {
							if (currentRule.Matches(symbols)) {
								rule = currentRule;
								return true;
							}
						}
					}
				}
			}
			rule = null;
			return false;
		}

		private readonly CompiledGrammar grammar;

		public RuleDeclarationParser(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			this.grammar = grammar;
		}

		public bool TryParse(string ruleDeclaration, out Rule rule) {
			Reduction ruleToken;
			if (TryParse(ruleDeclaration, out ruleToken)) {
				return TryBind(ruleToken, grammar, out rule);
			}
			rule = null;
			return false;
		}
	}
}