using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// Rule is the logical structures of the grammar.
	/// </summary>
	/// <remarks>
	/// Rules consist of a head containing a nonterminal 
	/// followed by a series of both nonterminals and terminals.
	/// </remarks>	
	public sealed class Rule: GrammarObject, ICollection<Symbol> {
		private Symbol head;
		private bool isOneNonTerminal;
		private Symbol[] symbols;

		/// <summary>
		/// Creates a new instance of <c>Rule</c> class.
		/// </summary>
		/// <param name="index">Index of the rule in the grammar rule table.</param>
		internal Rule(CompiledGrammar grammar, int index): base(grammar, index) {}

		/// <summary>
		/// Gets symbol by its index.
		/// </summary>
		public Symbol this[int index] {
			get {
				return symbols[index];
			}
		}

		/// <summary>
		/// Gets true if the rule contains exactly one symbol.
		/// </summary>
		/// <remarks>Used by the Parser object to TrimReductions</remarks>
		public bool ContainsOneNonTerminal {
			get {
				return isOneNonTerminal;
			}
		}

		/// <summary>
		/// Gets the rule definition.
		/// </summary>
		public string Definition {
			get {
				StringBuilder result = new StringBuilder();
				for (int i = 0; i < symbols.Length; i++) {
					result.Append(symbols[i].ToString());
					if (i < symbols.Length-1) {
						result.Append(' ');
					}
				}
				return result.ToString();
			}
		}

		/// <summary>
		/// Gets the head symbol of the rule.
		/// </summary>
		public Symbol Head {
			get {
				return head;
			}
		}

		/// <summary>
		/// Gets name of the rule.
		/// </summary>
		public string Name {
			get {
				return '<'+head.Name+'>';
			}
		}

		/// <summary>
		/// Gets number of symbols.
		/// </summary>
		public int SymbolCount {
			get {
				return symbols.Length;
			}
		}

		internal bool IsOneNonTerminal {
			get {
				return isOneNonTerminal;
			}
		}

		/// <summary>
		/// Checks if the symbols in this rule match the given symbols.
		/// </summary>
		/// <param name="symbols">The symbols.</param>
		/// <returns></returns>
		public bool Matches(ICollection<Symbol> symbols) {
			int index = 0;
			foreach (Symbol symbol in symbols) {
				if (symbol != null) {
					if (symbol != this.symbols[index++]) {
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the Backus-Naur representation of the rule.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Name+" ::= "+Definition;
		}

		/// <summary>
		/// Initializes the specified head.
		/// </summary>
		/// <param name="head">Nonterminal of the rule.</param>
		/// <param name="symbols">Terminal and nonterminal symbols of the rule.</param>
		internal void Initialize(Symbol head, Symbol[] symbols) {
			if (head == null) {
				throw new ArgumentNullException("head");
			}
			if (symbols == null) {
				throw new ArgumentNullException("symbols");
			}
			if (this.head != null) {
				throw new InvalidOperationException("The rule has already been initialized");
			}
			this.head = head;
			this.symbols = symbols;
			isOneNonTerminal = (symbols.Length == 1) && (symbols[0].Kind == SymbolType.NonTerminal);
		}

		IEnumerator<Symbol> IEnumerable<Symbol>.GetEnumerator() {
			return (IEnumerator<Symbol>)symbols.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return symbols.GetEnumerator();
		}

		void ICollection<Symbol>.Add(Symbol item) {
			throw new NotSupportedException();
		}

		void ICollection<Symbol>.Clear() {
			throw new NotSupportedException();
		}

		bool ICollection<Symbol>.Contains(Symbol item) {
			return Array.IndexOf(symbols, item) >= 0;
		}

		void ICollection<Symbol>.CopyTo(Symbol[] array, int arrayIndex) {
			symbols.CopyTo(array, Index);
		}

		bool ICollection<Symbol>.Remove(Symbol item) {
			throw new NotSupportedException();
		}

		int ICollection<Symbol>.Count {
			get {
				return symbols.Length;
			}
		}

		bool ICollection<Symbol>.IsReadOnly {
			get {
				return true;
			}
		}
	}
}