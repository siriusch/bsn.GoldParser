// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2014 by Arsène von Wyss - avw@gmx.ch
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

namespace bsn.GoldParser.Text {
	/// <summary>
	/// A style provider applies formatting styles to a <see cref="RichTextWriter"/> based on enumeration values.
	/// </summary>
	[CLSCompliant(false)]
	public interface IStyleProvider {
		/// <summary>
		/// Resets the style of the specified rich text writer.
		/// </summary>
		/// <param name="writer">The rich text writer.</param>
		void Reset(RichTextWriter writer);

		/// <summary>
		/// Sets the style on the rich text writer for a specific enumeration value.
		/// </summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="writer">The rich text writer.</param>
		/// <param name="style">The enumeration value to use for applying the style.</param>
		void Set<T>(RichTextWriter writer, T style) where T: struct, IComparable, IFormattable, IConvertible;
	}
}
