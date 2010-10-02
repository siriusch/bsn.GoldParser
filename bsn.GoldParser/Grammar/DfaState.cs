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
using System.Diagnostics;

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
		/// Gets the transition origin states.
		/// </summary>
		/// <returns></returns>
		public ICollection<DfaState> GetOriginStates() {
			return Owner.GetStateOrigins(this);
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

		/// <summary>
		/// Gets the transition destination states.
		/// </summary>
		/// <returns></returns>
		public ICollection<DfaState> GetTransitionStates() {
			return transitionVector.Values;
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
				int sequential = 0;
				char lastChar = '\0';
				foreach (char ch in owner.GetDfaCharset(edge.CharSetIndex).Characters) {
					if (lastChar == ch-1) {
						sequential++;
					} else {
						if(sequential > 3) {
							Debug.WriteLine(sequential, "Charset sequence detected");
						}
						sequential = 0;
					}
					lastChar = ch;
					transitionVector.Add(ch, targetDfaState);
				}
			}
		}
	}
}