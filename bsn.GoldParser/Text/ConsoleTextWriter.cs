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
using System.Drawing;
using System.Text;

namespace bsn.GoldParser.Text {
	/// <summary>
	/// A <see cref="RichTextWriter"/> implementation for writing to the console. Supports indentation and output colors.
	/// </summary>
	public class ConsoleTextWriter: RichTextWriter {
		internal static ConsoleColor GetConsoleColor(Color color) {
			if (color.GetSaturation() < 0.5) {
				// we have a grayish color
				switch ((int)(color.GetBrightness()*3.5)) {
				case 0:
					return ConsoleColor.Black;
				case 1:
					return ConsoleColor.DarkGray;
				case 2:
					return ConsoleColor.Gray;
				default:
					return ConsoleColor.White;
				}
			}
			int hue = (int)Math.Round(color.GetHue()/60, MidpointRounding.AwayFromZero);
			if (color.GetBrightness() < 0.4) {
				// dark color
				switch (hue) {
				case 1:
					return ConsoleColor.DarkYellow;
				case 2:
					return ConsoleColor.DarkGreen;
				case 3:
					return ConsoleColor.DarkCyan;
				case 4:
					return ConsoleColor.DarkBlue;
				case 5:
					return ConsoleColor.DarkMagenta;
				default:
					return ConsoleColor.DarkRed;
				}
			}
			// bright color
			switch (hue) {
			case 1:
				return ConsoleColor.Yellow;
			case 2:
				return ConsoleColor.Green;
			case 3:
				return ConsoleColor.Cyan;
			case 4:
				return ConsoleColor.Blue;
			case 5:
				return ConsoleColor.Magenta;
			default:
				return ConsoleColor.Red;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleTextWriter"/> class bound to the console.
		/// </summary>
		public ConsoleTextWriter(): this(null) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleTextWriter"/> class bound to the console, using the specified style provider.
		/// </summary>
		/// <param name="styleProvider">The style provider.</param>
		[CLSCompliant(false)]
		public ConsoleTextWriter(IStyleProvider styleProvider): base(null, styleProvider) {}

		public override Encoding Encoding {
			get {
				return Console.OutputEncoding;
			}
		}

		public override string NewLine {
			get {
				return Console.Out.NewLine;
			}
			set {
				Console.Out.NewLine = value;
			}
		}

		protected override bool IndentPending {
			get {
				return base.IndentPending || (Console.CursorLeft == 0);
			}
			set {
				base.IndentPending = value;
			}
		}

		public override void Reset() {
			Console.ResetColor();
		}

		public override void SetBackground(Color color) {
			Console.BackgroundColor = GetConsoleColor(color);
		}

		public override void SetForeground(Color color) {
			Console.ForegroundColor = GetConsoleColor(color);
		}

		protected override void WriteInternal(char value) {
			Console.Write(value);
		}

		protected override void WriteInternal(string value) {
			Console.Write(value);
		}

		protected override void WriteInternal(char[] buffer, int index, int count) {
			Console.Write(buffer, index, count);
		}
	}
}
