// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// A class for queue-like character buffering with the ability to mark positions and work with them.
	/// The class guarantees that all characters referred to by marks remain in memory.
	/// </summary>
	internal class CharBuffer {
		/// <summary>
		/// A character buffer mark, marking a position.
		/// </summary>
		/// <remarks>Make sure to dispose the mark when it is no longer needed. The <see cref="CharBuffer"/> will keep all characters belonging to the mark in memory until the mark is disposed.</remarks>
		public class Mark: IDisposable {
			private readonly CharBuffer owner;
			private int index;

			/// <summary>
			/// Initializes a new instance of the <see cref="Mark"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			protected internal Mark(CharBuffer owner) {
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				this.owner = owner;
				index = owner.tail;
				owner.marks.Add(this);
			}

			/// <summary>
			/// Gets the distance of this mark to the current position of the <see cref="Owner"/>.
			/// </summary>
			/// <value>The distance as number of characters.</value>
			public int Distance {
				get {
					AssertNotDisposed();
					return owner.tail-index;
				}
			}

			/// <summary>
			/// Gets the <see cref="CharBuffer"/> owning this mark.
			/// </summary>
			/// <value>The owner.</value>
			public CharBuffer Owner {
				get {
					return owner;
				}
			}

			/// <summary>
			/// Gets the position relative to the beginning of the text data.
			/// </summary>
			/// <value>The position.</value>
			public int Position {
				get {
					AssertNotDisposed();
					return owner.bufferPosition+index;
				}
			}

			/// <summary>
			/// Gets the text between the mark and the current read position of the <see cref="Owner"/>.
			/// </summary>
			/// <remarks>The text will be <see cref="Distance"/> characters long.</remarks>
			/// <value>The text.</value>
			public string Text {
				get {
					if (index < 0) {
						return string.Empty;
					}
					return GetText(null);
				}
			}

			/// <summary>
			/// Gets the start index relative to the current buffer of the <see cref="Owner"/>.
			/// </summary>
			/// <value>The index.</value>
			protected internal int Index {
				get {
					return index;
				}
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose() {
				Dispose(true);
			}

			/// <summary>
			/// Gets the offset between this mark and the other <paramref name="mark"/>.
			/// </summary>
			/// <param name="mark">The mark.</param>
			/// <returns>A positive number if this mark is more advanced than the given <paramref name="mark"/>.</returns>
			public int GetOffset(Mark mark) {
				AssertNotDisposed();
				if (mark == null) {
					throw new ArgumentNullException("mark");
				}
				if (mark.Owner != owner) {
					throw new ArgumentException("The mark is for a different buffer instance", "mark");
				}
				if (mark.Index < 0) {
					throw new ObjectDisposedException("mark");
				}
				return index-mark.index;
			}

			/// <summary>
			/// Gets the text between this mark and the given <paramref name="mark"/>.
			/// </summary>
			/// <param name="mark">The mark. If it is <c>null</c>, the method will return the text to the current read position of the <see cref="Owner"/>.</param>
			/// <returns>The text.</returns>
			public string GetText(Mark mark) {
				if (mark == null) {
					return new String(owner.buffer, index, Distance);
				}
				return new string(owner.buffer, (mark.index < index) ? mark.index : index, Math.Abs(GetOffset(mark)));
			}

			/// <summary>
			/// Moves the mark to the current read position of the <see cref="Owner"/>.
			/// </summary>
			public void MoveToReadPosition() {
				AssertNotDisposed();
				if (index != owner.tail) {
					owner.marks.Remove(this);
					index = owner.tail;
					owner.marks.Add(this);
				}
			}

			public override string ToString() {
				return Text;
			}

			/// <summary>
			/// Asserts the mark has not been disposed.
			/// </summary>
			/// <exception cref="ObjectDisposedException">When the mark has already been disposed.</exception>
			protected void AssertNotDisposed() {
				if (index < 0) {
					throw new ObjectDisposedException("Mark");
				}
			}

			/// <summary>
			/// Releases unmanaged and - optionally - managed resources
			/// </summary>
			/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
			protected virtual void Dispose(bool disposing) {
				if (disposing) {
					owner.marks.Remove(this);
					index = -1;
				}
			}

			internal void Relocate(int offset) {
				Debug.Assert((offset > 0) && (offset <= index));
				index -= offset;
			}
		}

		private readonly List<Mark> marks = new List<Mark>(10);
		private readonly TextReader reader;
		private char[] buffer = new char[1024];
		private int bufferPosition;
		private bool endReached;
		private int head;
		private int tail;

		/// <summary>
		/// Initializes a new instance of the <see cref="CharBuffer"/> class.
		/// </summary>
		/// <param name="reader">The reader to use.</param>
		public CharBuffer(TextReader reader) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			this.reader = reader;
			endReached = !FillBuffer();
		}

		/// <summary>
		/// Gets the <see cref="System.Char"/> st the specified position.
		/// </summary>
		/// <remarks>The range of available characters is dynamic. Only by using <see cref="Mark"/>s the availability of characters can be guaranteed.</remarks>
		public char this[int position] {
			get {
				int index = position-bufferPosition;
				if ((index < 0) || (index >= head)) {
					throw new ArgumentOutOfRangeException("position");
				}
				return buffer[index];
			}
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <value>The position.</value>
		public int Position {
			get {
				return bufferPosition+tail;
			}
		}

		/// <summary>
		/// Gets the text reader.
		/// </summary>
		/// <value>The text reader.</value>
		public TextReader TextReader {
			get {
				return reader;
			}
		}

		/// <summary>
		/// Gets the count of active marks on this buffer.
		/// </summary>
		/// <value>The mark count.</value>
		protected int MarkCount {
			get {
				return marks.Count;
			}
		}

		/// <summary>
		/// Creates a new mark.
		/// </summary>
		/// <remarks>Dispose the mark when it is no longer needed.</remarks>
		public virtual Mark CreateMark() {
			return new Mark(this);
		}

		/// <summary>
		/// Moves the read position to the given <paramref name="mark"/>.
		/// </summary>
		/// <param name="mark">The mark.</param>
		public void MoveToMark(Mark mark) {
			if (mark == null) {
				throw new ArgumentNullException("mark");
			}
			if (mark.Owner != this) {
				throw new ArgumentException("The mark is for a different buffer instance", "mark");
			}
			if (mark.Index < 0) {
				throw new ObjectDisposedException("mark");
			}
			tail = mark.Index;
			foreach (Mark item in marks) {
				Debug.Assert(item.Index <= tail);
			}
		}

		/// <summary>
		/// Steps back in the buffer (read position).
		/// </summary>
		/// <remarks>Stepping back is only allowed in the range up to the leftmost mark.</remarks>
		/// <param name="count">The count.</param>
		public void StepBack(int count) {
			AssertMinimumMarkDistance(count);
			if ((count < 0) || (count > tail)) {
				throw new ArgumentOutOfRangeException("count");
			}
			tail -= count;
		}

		/// <summary>
		/// Tries to read the next char.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <returns><c>true</c> if successful, <c>false</c> if at the end of the reader.</returns>
		public bool TryReadChar(out char result) {
			if (tail == head) {
				// we are either at the beginning or at the end
				if (endReached) {
					result = '\x0';
					return false;
				}
			}
			result = buffer[tail++];
			if (tail == head) {
				endReached = !FillBuffer();
			}
			return true;
		}

		/// <summary>
		/// Asserts the minimum mark distance (Debug only).
		/// </summary>
		/// <param name="count">The count.</param>
		[Conditional("DEBUG")]
		protected void AssertMinimumMarkDistance(int count) {
			if (marks.Count == 0) {
				throw new ArgumentOutOfRangeException("count", "A mark has to be defined in order to enable look behind in the char buffer");
			}
			if (marks[0].Distance < count) {
				throw new ArgumentOutOfRangeException("count", "The look behind must not be greater than the first mark position");
			}
		}

		private bool FillBuffer() {
			Debug.Assert(head == tail, "The buffer cannot be filled if not finished reading");
			if (marks.Count == 0) {
				// the easy case: buffer is empty and we have no mark
				tail = 0;
				head = reader.ReadBlock(buffer, 0, buffer.Length);
				bufferPosition += head;
			} else {
				int tailMark = marks[0].Index;
				if (tailMark > 0) {
					// we have a mark, and it is not on position 0 -> relocate marked data
					char[] dst = buffer;
					if ((tailMark+64) >= buffer.Length) {
						// the tail (almost) fills the whole buffer, therefore grow the buffer
						dst = new char[buffer.Length*2];
					}
					head -= tailMark;
					Array.Copy(buffer, tailMark, dst, 0, head);
					tail = head;
					foreach (Mark mark in marks) {
						// relocate all marks in order to keep their index in sync
						mark.Relocate(tailMark);
					}
					buffer = dst; // replace the buffer, it may have been grown
				}
				int read = reader.ReadBlock(buffer, head, buffer.Length-head); // fill buffer
				head += read;
			}
			return (head > tail);
		}
	}
}