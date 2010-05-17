// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.CompilerServices;

namespace bsn.GoldParser.Grammar {
	public abstract class GrammarObject<TSelf>: IEquatable<TSelf> where TSelf: GrammarObject<TSelf> {
		private readonly int index;
		private readonly CompiledGrammar owner;

		protected GrammarObject(CompiledGrammar owner, int index) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
			this.index = index;
		}

		/// <summary>
		/// Gets index of the LR state action.
		/// </summary>
		public int Index {
			get {
				return index;
			}
		}

		public CompiledGrammar Owner {
			get {
				return owner;
			}
		}

		public bool Equals(TSelf other) {
			return ReferenceEquals(this, other);
		}

		public sealed override int GetHashCode() {
			return base.GetHashCode();
		}

		public sealed override bool Equals(object other) {
			return ReferenceEquals(this, other);
		}
	}
}