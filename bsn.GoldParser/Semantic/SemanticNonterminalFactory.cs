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

using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// The abstract nongeneric case class for semantic nonterminal tokens. This class is for internal use only.
	/// </summary>
	public abstract class SemanticNonterminalFactory: SemanticTokenFactory {
		public abstract ReadOnlyCollection<Type> InputTypes {
			get;
		}

		protected internal abstract IEnumerable<Symbol> GetInputSymbols(Rule rule);

		internal abstract SemanticToken CreateInternal(Rule rule, IList<SemanticToken> tokens);
	}

	/// <summary>
	/// The abstract generic case class for semantic nonterminal tokens. This class is usually not directly inherited.
	/// </summary>
	/// <typeparam name="T">The type of the nonterminal token.</typeparam>
	public abstract class SemanticNonterminalFactory<T>: SemanticNonterminalFactory where T: SemanticToken {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		public abstract T Create(Rule rule, IList<SemanticToken> tokens);

		protected internal override IEnumerable<Symbol> GetInputSymbols(Rule rule) {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			return rule;
		}

		internal override sealed SemanticToken CreateInternal(Rule rule, IList<SemanticToken> tokens) {
			return Create(rule, tokens);
		}
	}
}