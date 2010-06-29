using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.GoldParser.Semantic {
	public abstract class SemanticActions<T> where T: SemanticToken {
		private class SemanticTokenizer: Tokenizer<SemanticToken> {
			private readonly SemanticActions<T> actions;

			public SemanticTokenizer(TextReader textReader, SemanticActions<T> actions): base(textReader, actions.Grammar) {
				this.actions = actions;
			}

			protected override SemanticToken CreateToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
				return actions.CreateTerminalToken(tokenSymbol, tokenPosition, text);
			}
		}

		internal static CompiledGrammar GetGrammar(SemanticActions<T> actions) {
			if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			return actions.Grammar;
		}

		private readonly CompiledGrammar grammar;
		private readonly Dictionary<Rule, SemanticNonterminalFactory> nonterminalFactories = new Dictionary<Rule, SemanticNonterminalFactory>();
		private readonly object sync = new object();
		private readonly Dictionary<Symbol, SemanticTerminalFactory> terminalFactories = new Dictionary<Symbol, SemanticTerminalFactory>();
		private readonly Dictionary<Rule, SemanticTrimFactory<T>> trimFactories = new Dictionary<Rule, SemanticTrimFactory<T>>();
		private bool initialized;

		protected SemanticActions(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			this.grammar = grammar;
			for (int i = 0; i < grammar.RuleCount; i++) {
				Rule rule = grammar.GetRule(i);
				if (rule.SymbolCount == 1) {
					trimFactories.Add(rule, new SemanticTrimFactory<T>(this, rule, 0));
				}
			}
		}

		public IEnumerable<SemanticTokenFactory> AllTokenFactories {
			get {
				foreach (KeyValuePair<Symbol, SemanticTerminalFactory> terminalFactory in terminalFactories) {
					yield return terminalFactory.Value;
				}
				foreach (KeyValuePair<Rule, SemanticNonterminalFactory> nonterminalFactory in nonterminalFactories) {
					yield return nonterminalFactory.Value;
				}
				foreach (KeyValuePair<Rule, SemanticTrimFactory<T>> trimFactory in trimFactories) {
					if (!nonterminalFactories.ContainsKey(trimFactory.Key)) {
						yield return trimFactory.Value;
					}
				}
			}
		}
		public CompiledGrammar Grammar {
			get {
				return grammar;
			}
		}
		protected internal bool Initialized {
			get {
				return initialized;
			}
		}

		protected object SyncRoot {
			get {
				return sync;
			}
		}

		public abstract Type GetSymbolOutputType(Symbol symbol);

		public void Initialize() {
			lock (sync) {
				if (!initialized) {
					InitializeInternal();
					initialized = true;
					CheckConsistency();
				}
			}
		}

		public bool TryGetNonterminalFactory(Rule rule, out SemanticNonterminalFactory factory) {
			Initialize();
			if (nonterminalFactories.TryGetValue(rule, out factory)) {
				return true;
			}
			SemanticTrimFactory<T> trimFactory;
			if (trimFactories.TryGetValue(rule, out trimFactory)) {
				factory = trimFactory;
				return true;
			}
			return false;
		}

		public bool TryGetTerminalFactory(Symbol symbol, out SemanticTerminalFactory factory) {
			Initialize();
			return terminalFactories.TryGetValue(symbol, out factory);
		}

		protected internal virtual ITokenizer<SemanticToken> CreateTokenizer(TextReader reader) {
			Initialize();
			return new SemanticTokenizer(reader, this);
		}

		protected void AssertNotInitialized() {
			if (initialized) {
				throw new InvalidOperationException("The object is already initialized");
			}
		}

		protected void CheckConsistency() {
			List<string> errors = new List<string>();
			SymbolTypeMap<T> symbolTypes = new SymbolTypeMap<T>();
			// step 1: check that all terminals have a factory and register their output type
			for (int i = 0; i < grammar.SymbolCount; i++) {
				Symbol symbol = grammar.GetSymbol(i);
				if (symbol.Kind != SymbolKind.Nonterminal) {
					SemanticTerminalFactory factory;
					if (TryGetTerminalFactory(symbol, out factory)) {
						Debug.WriteLine(factory.OutputType.FullName, symbol.ToString());
						symbolTypes.SetTypeForSymbol(symbol, factory.OutputType);
					} else {
						errors.Add(String.Format("Semantic token is missing for terminal {0}", symbol));
					}
				}
			}
			// step 2: check that all rules have a factory and register their output type
			for (int i = 0; i < grammar.RuleCount; i++) {
				Rule rule = grammar.GetRule(i);
				SemanticNonterminalFactory factory;
				if (TryGetNonterminalFactory(rule, out factory)) {
					Debug.WriteLine(factory.OutputType.FullName, rule.RuleSymbol.ToString());
					symbolTypes.SetTypeForSymbol(rule.RuleSymbol, factory.OutputType);
				} else {
					errors.Add(String.Format("Semantic token is missing for rule {0}", rule));
				}
			}
			// step 3: check the input types of all rules
			foreach (KeyValuePair<Rule, SemanticNonterminalFactory> pair in nonterminalFactories) {
				ReadOnlyCollection<Type> inputTypes = pair.Value.InputTypes;
				int index = 0;
				foreach (Symbol inputSymbol in pair.Value.GetInputSymbols(pair.Key)) {
					Type handleType = symbolTypes.GetSymbolType(inputSymbol);
					if (!inputTypes[index].IsAssignableFrom(handleType)) {
						errors.Add(string.Format("The factory for the type {0} used by rule {1} expects a {2} on index {3}, but receives a {4}", pair.Value.OutputType.FullName, pair.Key.Definition, inputTypes[index].FullName, index, handleType.FullName));
					}
					index++;
				}
				if (index != inputTypes.Count) {
					errors.Add(string.Format("The factory for the type {0} used by rule {1} has a mismatch of input symbol count ({2}) and type count ({3})", pair.Value.OutputType.FullName, pair.Key.Definition, index, inputTypes.Count));
				}
			}
			// throw if errors were found
			if (errors.Count > 0) {
				StringBuilder result = new StringBuilder();
				result.AppendLine("The semantic engine found errors:");
				foreach (string error in errors) {
					result.AppendLine(error);
				}
				throw new InvalidOperationException(result.ToString());
			}
		}

		protected IEnumerable<SemanticTokenFactory> GetTokenFactoriesForSymbol(Symbol symbol) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Kind == SymbolKind.Nonterminal) {
				foreach (Rule rule in grammar.GetRulesForSymbol(symbol)) {
					SemanticNonterminalFactory nonterminalFactory;
					if (TryGetNonterminalFactory(rule, out nonterminalFactory)) {
						yield return nonterminalFactory;
					}
				}
			} else {
				SemanticTerminalFactory terminalFactory;
				if (TryGetTerminalFactory(symbol, out terminalFactory)) {
					yield return terminalFactory;
				}
			}
		}

		protected abstract void InitializeInternal();

		protected virtual void RegisterNonterminalFactory(Rule rule, SemanticNonterminalFactory factory) {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if (rule.Owner != grammar) {
				throw new ArgumentException("The rule was defined on another grammar", "rule");
			}
			if (factory == null) {
				throw new ArgumentNullException("factory");
			}
			AssertNotInitialized();
			nonterminalFactories.Add(rule, factory);
		}

		protected virtual void RegisterTerminalFactory(Symbol symbol, SemanticTerminalFactory factory) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != grammar) {
				throw new ArgumentException("The symbol was defined on another grammar", "symbol");
			}
			if (symbol.Kind == SymbolKind.Nonterminal) {
				throw new ArgumentException("Terminal symbol factories can only build terminals and special symbols", "symbol");
			}
			if (factory == null) {
				throw new ArgumentNullException("factory");
			}
			AssertNotInitialized();
			terminalFactories.Add(symbol, factory);
		}

		private SemanticToken CreateTerminalToken(Symbol tokenSymbol, LineInfo tokenPosition, string text) {
			SemanticToken result = terminalFactories[tokenSymbol].CreateInternal(text);
			result.Initialize(tokenSymbol, tokenPosition);
			return result;
		}
	}
}
