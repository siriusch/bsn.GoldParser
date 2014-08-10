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
using System.IO;
using System.Text;

namespace bsn.GoldParser.Text {
	/// <summary>
	/// The RichTextWriter is an extended <see cref="TextWriter" /> which adds support for basic formatting and indentation, such as typically used for syntax highlighting.
	/// </summary>
	public abstract class RichTextWriter: TextWriter {
		private class DelegateWriter: RichTextWriter {
			private readonly TextWriter writer;

			public DelegateWriter(TextWriter writer, IStyleProvider styleProvider): base(null, styleProvider) {
				this.writer = writer;
			}

			public override Encoding Encoding {
				get {
					return writer.Encoding;
				}
			}

			public override IFormatProvider FormatProvider {
				get {
					return writer.FormatProvider;
				}
			}

			public override string NewLine {
				get {
					return writer.NewLine;
				}
				set {
					writer.NewLine = value;
				}
			}

			public override void Close() {
				writer.Close();
			}

			public override void Flush() {
				writer.Flush();
			}

			public override string ToString() {
				return writer.ToString();
			}

			protected override void Dispose(bool disposing) {
				if (disposing) {
					writer.Dispose();
				}
			}

			protected override void WriteInternal(char value) {
				writer.Write(value);
			}

			protected override void WriteInternal(char[] buffer, int index, int count) {
				writer.Write(buffer, index, count);
			}

			protected override void WriteInternal(string value) {
				writer.Write(value);
			}
		}

		private class IndentInfo: IDisposable {
			private readonly int depth;
			private readonly IndentInfo previous;
			private RichTextWriter owner;

			public IndentInfo(RichTextWriter owner, int depth) {
				this.owner = owner;
				this.depth = depth;
				previous = owner.indent;
			}

			public int Depth {
				get {
					return depth;
				}
			}

			public void Dispose() {
				if (owner != null) {
					try {
						if (owner.indent != this) {
							throw new InvalidOperationException("The indenting does not match");
						}
						owner.indent = previous;
					} finally {
						owner = null;
					}
				}
			}
		}

		private class NullWriter: RichTextWriter {
			public override Encoding Encoding {
				get {
					return Null.Encoding;
				}
			}

			protected override void WriteInternal(char value) {}

			protected override void WriteInternal(char[] buffer, int index, int count) {}

			protected override void WriteInternal(string value) {}
		}

		private static readonly NullWriter nullWriter = new NullWriter();

		/// <summary>
		/// Gets the null RichTextWriter.
		/// </summary>
		public new static RichTextWriter Null {
			get {
				return nullWriter;
			}
		}

		/// <summary>
		/// Wraps the specified writer with a RichTextWriter, adding indentation support on the way.
		/// </summary>
		/// <param name="writer">The text writer to wrap.</param>
		/// <returns>The wrapped TextWriter.</returns>
		public static RichTextWriter Wrap(TextWriter writer) {
			return new DelegateWriter(writer, null);
		}

		private readonly IStyleProvider styleProvider;
		private IndentInfo indent;
		private string indentChars;
		private bool indentPending;

		[CLSCompliant(false)] 
		protected RichTextWriter(IFormatProvider formatProvider, IStyleProvider styleProvider): base(formatProvider) {
			this.styleProvider = styleProvider;
			indentPending = true;
		}

		protected RichTextWriter() {}

		/// <summary>
		/// Gets or sets the indentation chars.
		/// </summary>
		/// <remarks>If not specified (or set to <c>null</c>), two spaces are used.</remarks>
		public string IndentChars {
			get {
				return indentChars ?? "  ";
			}
			set {
				indentChars = value;
			}
		}

		/// <summary>
		/// Gets the style provider provided in the constructor of this rich text writer.
		/// </summary>
		/// <value>
		/// The style provider.
		/// </value>
		[CLSCompliant(false)]
		public IStyleProvider StyleProvider {
			get {
				return styleProvider;
			}
		}

		protected int IndentDepth {
			get {
				if (indent == null) {
					return 0;
				}
				return indent.Depth;
			}
		}

		protected virtual bool IndentPending {
			get {
				return indentPending;
			}
			set {
				indentPending = value;
			}
		}

		/// <summary>
		/// Increments the indentation and returns a disposable handle.
		/// </summary>
		/// <remarks>The indentation is decremented by disposing the handle.</remarks>
		/// <returns>The indentation handle.</returns>
		public IDisposable Indent() {
			return Indent(1);
		}

		/// <summary>
		/// Increments the indentation by the specified depth and returns a disposable handle.
		/// </summary>
		/// <remarks>The indentation is decremented by disposing the handle.</remarks>
		/// <returns>The indentation handle.</returns>
		public IDisposable Indent(int depth) {
			if (depth < 0) {
				throw new ArgumentOutOfRangeException("depth", "Negative indent is not allowed");
			}
			indent = new IndentInfo(this, IndentDepth+depth);
			return indent;
		}

		/// <summary>
		/// Resets the style of the writer.
		/// </summary>
		public virtual void Reset() {}

		/// <summary>
		/// Sets the background color of this writer.
		/// </summary>
		/// <param name="color">The color.</param>
		public virtual void SetBackground(Color color) {}

		/// <summary>
		/// Controls the boldness of the font.
		/// </summary>
		public virtual void SetBold(bool bold) {}

		/// <summary>
		/// Sets the foreground color of this writer.
		/// </summary>
		/// <param name="color">The color.</param>
		public virtual void SetForeground(Color color) {}

		/// <summary>
		/// Controls the italic sytle of the font.
		/// </summary>
		public virtual void SetItalic(bool bold) {}

		/// <summary>
		/// Apply a style by using the configured <see cref="StyleProvider"/>.
		/// </summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="style">The enumeration value specifying the style to use.</param>
		/// <remarks>This has no effect if no style provider has been set.</remarks>
		[CLSCompliant(false)]
		public void SetStyle<T>(T style) where T: struct, IComparable, IFormattable, IConvertible {
			if (styleProvider != null) {
				styleProvider.Set(this, style);
			}
		}

		public override sealed void Write(char value) {
			AssertIndentation();
			WriteInternal(value);
		}

		public override sealed void Write(char[] buffer) {
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length > 0) {
				AssertIndentation();
				WriteInternal(buffer, 0, buffer.Length);
			}
		}

		public override sealed void Write(char[] buffer, int index, int count) {
			if (count > 0) {
				AssertIndentation();
				WriteInternal(buffer, index, count);
			}
		}

		public override sealed void Write(bool value) {
			base.Write(value);
		}

		public override sealed void Write(int value) {
			base.Write(value);
		}

		[CLSCompliant(false)]
		public override sealed void Write(uint value) {
			base.Write(value);
		}

		public override sealed void Write(long value) {
			base.Write(value);
		}

		[CLSCompliant(false)]
		public override sealed void Write(ulong value) {
			base.Write(value);
		}

		public override sealed void Write(float value) {
			base.Write(value);
		}

		public override sealed void Write(double value) {
			base.Write(value);
		}

		public override sealed void Write(decimal value) {
			base.Write(value);
		}

		public override sealed void Write(string value) {
			if (!string.IsNullOrEmpty(value)) {
				AssertIndentation();
				WriteInternal(value);
			}
		}

		public override sealed void Write(object value) {
			base.Write(value);
		}

		public override sealed void Write(string format, object arg0) {
			base.Write(format, arg0);
		}

		public override sealed void Write(string format, object arg0, object arg1) {
			base.Write(format, arg0, arg1);
		}

		public override sealed void Write(string format, object arg0, object arg1, object arg2) {
			base.Write(format, arg0, arg1, arg2);
		}

		// ReSharper disable MethodOverloadWithOptionalParameter
		public override sealed void Write(string format, params object[] arg) {
			base.Write(format, arg);
		}
		// ReSharper restore MethodOverloadWithOptionalParameter

		public override sealed void WriteLine() {
			WriteInternal(NewLine);
			IndentPending = true;
		}

		public override sealed void WriteLine(char value) {
			Write(value);
			WriteLine();
		}

		public override sealed void WriteLine(char[] buffer) {
			Write(buffer);
			WriteLine();
		}

		public override sealed void WriteLine(char[] buffer, int index, int count) {
			Write(buffer, index, count);
			WriteLine();
		}

		public override sealed void WriteLine(bool value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(int value) {
			base.WriteLine(value);
		}

		[CLSCompliant(false)]
		public override sealed void WriteLine(uint value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(long value) {
			base.WriteLine(value);
		}

		[CLSCompliant(false)]
		public override sealed void WriteLine(ulong value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(float value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(double value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(decimal value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(string value) {
			Write(value);
			WriteLine();
		}

		public override sealed void WriteLine(object value) {
			base.WriteLine(value);
		}

		public override sealed void WriteLine(string format, object arg0) {
			base.WriteLine(format, arg0);
		}

		public override sealed void WriteLine(string format, object arg0, object arg1) {
			base.WriteLine(format, arg0, arg1);
		}

		public override sealed void WriteLine(string format, object arg0, object arg1, object arg2) {
			base.WriteLine(format, arg0, arg1, arg2);
		}

		// ReSharper disable MethodOverloadWithOptionalParameter
		public override sealed void WriteLine(string format, params object[] arg) {
			base.WriteLine(format, arg);
		}
		// ReSharper restore MethodOverloadWithOptionalParameter

		protected void AssertIndentation() {
			if (IndentPending) {
				IndentPending = false;
				WriteIndentation();
			}
		}

		protected virtual void WriteIndentation() {
			int depth = IndentDepth;
			for (int i = 0; i < depth; i++) {
				WriteInternal(IndentChars);
			}
		}

		protected abstract void WriteInternal(char value);

		protected virtual void WriteInternal(string value) {
			if (!string.IsNullOrEmpty(value)) {
				char[] buffer = value.ToCharArray();
				WriteInternal(buffer, 0, buffer.Length);
			}
		}

		protected virtual void WriteInternal(char[] buffer, int index, int count) {
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			if (index < 0) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException("count");
			}
			int end = index+count;
			if (end > buffer.Length) {
				throw new ArgumentException("Count characters after indesx exceeds buffer length");
			}
			while (index < end) {
				WriteInternal(buffer[index++]);
			}
		}
	}
}
