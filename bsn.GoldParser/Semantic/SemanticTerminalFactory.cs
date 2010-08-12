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

namespace bsn.GoldParser.Semantic {
	/// <summary>
	/// The abstract nongeneric case class for semantic terminal tokens. This class is for internal use only.
	/// </summary>
	public abstract class SemanticTerminalFactory: SemanticTokenFactory {
		internal SemanticTerminalFactory() {}

		internal abstract SemanticToken CreateInternal(string text);
	}

	/// <summary>
	/// The abstract generic case class for semantic terminal tokens. This class is usually not directly inherited.
	/// </summary>
	/// <typeparam name="T">The type of the terminal token.</typeparam>
	public abstract class SemanticTerminalFactory<T>: SemanticTerminalFactory where T: SemanticToken {
		public override sealed Type OutputType {
			get {
				return typeof(T);
			}
		}

		protected abstract T Create(string text);

		internal override sealed SemanticToken CreateInternal(string text) {
			return Create(text);
		}
	}
}