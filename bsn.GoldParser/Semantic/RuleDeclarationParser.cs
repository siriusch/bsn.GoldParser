using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

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

		private readonly CompiledGrammar grammar;

		public RuleDeclarationParser(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			this.grammar = grammar;
		}

		public bool TryParse(string ruleDeclaration, out Rule rule) {
			using (StringReader reader = new StringReader(ruleDeclaration)) {
				LalrProcessor processor = new LalrProcessor(new Tokenizer(reader, ruleGrammar));
				ParseMessage message;
				do {
					message = processor.Parse();
					if (message == ParseMessage.Accept) {
						Reduction ruleDecl = (Reduction)processor.CurrentToken;
						Symbol ruleSymbol;
						if (grammar.TryGetSymbol(ruleDecl.Children[0].Text, out ruleSymbol)) {
							ReadOnlyCollection<Rule> rules;
							if (grammar.TryGetRulesForSymbol(ruleSymbol, out rules)) {
								List<Symbol> symbols = new List<Symbol>();
								Reduction handle = (Reduction)ruleDecl.Children[2];
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
					}
				} while (CompiledGrammar.CanContinueParsing(message));
			}
			rule = null;
			return false;
		}
	}
}
