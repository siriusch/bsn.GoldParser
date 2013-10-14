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
using System.Text.RegularExpressions;

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// This class is used to decorate constructors which accept exactly one string for the terminal value
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
	public sealed class TerminalAttribute: Attribute, IGrammarBindableAttribute<Symbol>, IEquatable<TerminalAttribute> {
		private static readonly Regex rxSpecialToken = new Regex(@"^\(.*\)$");
		private readonly Type[] genericTypeParameters;

		private readonly string symbolName;
		private object[] arguments;

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminalAttribute" /> class.
		/// </summary>
		/// <param name="symbolName">Name of the symbol.</param>
		public TerminalAttribute(string symbolName): this(symbolName, null) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TerminalAttribute" /> class.
		/// </summary>
		/// <param name="symbolName">Name of the symbol.</param>
		/// <param name="genericTypes">The generic types.</param>
		/// <exception cref="System.ArgumentNullException">symbolName</exception>
		public TerminalAttribute(string symbolName, params Type[] genericTypes) {
			if (string.IsNullOrEmpty(symbolName)) {
				throw new ArgumentNullException("symbolName");
			}
			this.symbolName = rxSpecialToken.IsMatch(symbolName) ? symbolName : Symbol.FormatTerminalSymbol(symbolName);
			genericTypeParameters = genericTypes ?? Type.EmptyTypes;
		}

		/// <summary>
		/// Gets or sets the arguments.
		/// </summary>
		/// <value>
		/// The arguments.
		/// </value>
		public object[] Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value;
			}
		}

		/// <summary>
		/// Gets the name of the symbol.
		/// </summary>
		/// <value>
		/// The name of the symbol.
		/// </value>
		public string SymbolName {
			get {
				return symbolName;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj) {
			return Equals(obj as TerminalAttribute);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			unchecked {
				return (base.GetHashCode()*397)^(symbolName != null ? symbolName.GetHashCode() : 0);
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="TerminalAttribute" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="TerminalAttribute" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="TerminalAttribute" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(TerminalAttribute other) {
			if (ReferenceEquals(null, other)) {
				return false;
			}
			if (ReferenceEquals(this, other)) {
				return true;
			}
			return base.Equals(other) && string.Equals(symbolName, other.symbolName);
		}

		/// <summary>
		/// Gets the generic type parameters.
		/// </summary>
		/// <value>The generic type parameters.</value>
		public Type[] GenericTypeParameters {
			get {
				return genericTypeParameters;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is instantiating a generic type.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is instantiating a generic type; otherwise, <c>false</c>.
		/// </value>
		public bool IsGeneric {
			get {
				return genericTypeParameters.Length > 0;
			}
		}

		/// <summary>
		/// Binds the symbol to the specified grammar.
		/// </summary>
		/// <param name="grammar">The grammar.</param>
		/// <returns>The symbol matching the terminal attribute, or <c>null</c> if not found.</returns>
		/// <exception cref="System.ArgumentNullException">grammar</exception>
		public Symbol Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Symbol result;
			grammar.TryGetSymbol(symbolName, out result);
			return result;
		}
	}
}
