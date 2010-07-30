// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	/// <summary>
	/// A generic interface for tokenizers.
	/// </summary>
	/// <remarks>
	/// The <see cref="LalrProcessor{T}"/> instances accept any tokenizer implementing this interface, but usually a tokenizer derived from the default implementation <see cref="Tokenizer{T}"/> is used.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public interface ITokenizer<T> {
		/// <summary>
		/// Gets the grammar used by the tokenizer.
		/// </summary>
		/// <value>The compiled grammar.</value>
		CompiledGrammar Grammar {
			get;
		}

		/// <summary>
		/// Tries to read and tokenize the next token.
		/// </summary>
		/// <param name="token">The new token.</param>
		/// <returns>A parsing result.</returns>
		ParseMessage NextToken(out T token);
	}
}