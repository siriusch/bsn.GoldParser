// (C) 2010 Arsène von Wyss / bsn
using System;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// The base class for the grammar objects <see cref="DfaState"/>, <see cref="LalrAction"/>, <see cref="LalrState"/>, <see cref="Rule"/> and <see cref="Symbol"/>.
	/// </summary>
	/// <typeparam name="TSelf">The type of the grammar object implemented (self).</typeparam>
	public abstract class GrammarObject<TSelf>: IEquatable<TSelf> where TSelf: GrammarObject<TSelf> {
		private readonly int index;
		private readonly CompiledGrammar owner;

		/// <summary>
		/// Initializes a new instance of the <see cref="GrammarObject&lt;TSelf&gt;"/> class.
		/// </summary>
		/// <param name="owner">The owner grammar.</param>
		/// <param name="index">The index.</param>
		protected GrammarObject(CompiledGrammar owner, int index) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
			this.index = index;
		}

		/// <summary>
		/// Gets index of grammar object in the <see cref="CompiledGrammar"/>.
		/// </summary>
		public int Index {
			get {
				return index;
			}
		}

		/// <summary>
		/// Gets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public CompiledGrammar Owner {
			get {
				return owner;
			}
		}

		public override sealed bool Equals(object other) {
			return ReferenceEquals(this, other);
		}

		public override sealed int GetHashCode() {
			return base.GetHashCode();
		}

		public bool Equals(TSelf other) {
			return ReferenceEquals(this, other);
		}
	}
}