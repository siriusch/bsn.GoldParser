// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
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
				Tokenizer tokenizer = new Tokenizer(reader, ruleGrammar);
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
			if (grammar.TryGetSymbol(GetRuleSymbolName(ruleDeclaration), out ruleSymbol)) {
				ReadOnlyCollection<Rule> rules;
				if (grammar.TryGetRulesForSymbol(ruleSymbol, out rules)) {
					List<Symbol> symbols = new List<Symbol>();
					foreach (string handleName in GetRuleHandleNames(ruleDeclaration)) {
						Symbol symbol;
						if (!grammar.TryGetSymbol(handleName, out symbol)) {
							symbols = null;
							break;
						}
						symbols.Add(symbol);
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

		internal static string GetRuleSymbolName(Reduction ruleDeclaration) {
			if (ruleDeclaration == null) {
				throw new ArgumentNullException("ruleDeclaration");
			}
			return ruleDeclaration.Children[0].ToString();
		}

		internal static IEnumerable<string> GetRuleHandleNames(Reduction ruleDeclaration) {
			if (ruleDeclaration == null) {
				throw new ArgumentNullException("ruleDeclaration");
			}
			Reduction handle = (Reduction)ruleDeclaration.Children[2];
			while (handle.Children.Count == 2) {
				yield return handle.Children[0].ToString();
				handle = (Reduction)handle.Children[1];
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
			Reduction ruleToken;
			if (TryParse(ruleDeclaration, out ruleToken)) {
				return TryBind(ruleToken, grammar, out rule);
			}
			rule = null;
			return false;
		}
	}
}