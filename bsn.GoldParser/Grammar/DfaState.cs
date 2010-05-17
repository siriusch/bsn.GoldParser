// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// State in the Deterministic Finite Automata 
	/// which is used by the tokenizer.
	/// </summary>
	public sealed class DfaState: GrammarObject<DfaState> {
		private readonly Dictionary<char, DfaState> transitionVector = new Dictionary<char, DfaState>();
		private Symbol acceptSymbol;

		/// <summary>
		/// Creates a new instance of the <c>DfaState</c> class.
		/// </summary>
		internal DfaState(CompiledGrammar owner, int index): base(owner, index) {}

		/// <summary>
		/// Gets the symbol which can be accepted in this DFA state.
		/// </summary>
		public Symbol AcceptSymbol {
			get {
				return acceptSymbol;
			}
		}

		/// <summary>
		/// Gets the transition for the given character.
		/// </summary>
		/// <param name="ch">The ch.</param>
		/// <returns>The transition or null if there is not transition defined.</returns>
		public DfaState GetTransition(char ch) {
			DfaState result;
			if (transitionVector.TryGetValue(ch, out result)) {
				return result;
			}
			return null;
		}

		internal void Initialize(CompiledGrammar owner, Symbol acceptSymbol, CompiledGrammar.DfaEdge[] edges) {
			if (edges == null) {
				throw new ArgumentNullException("edges");
			}
			if ((this.acceptSymbol != null) || (transitionVector.Count > 0)) {
				throw new InvalidOperationException("The DfaState has already been initialized!");
			}
			this.acceptSymbol = acceptSymbol;
			foreach (CompiledGrammar.DfaEdge edge in edges) {
				DfaState targetDfaState = owner.GetDfaState(edge.TargetIndex);
				foreach (char ch in owner.GetDfaCharset(edge.CharSetIndex)) {
					transitionVector.Add(ch, targetDfaState);
				}
			}
		}
	}
}