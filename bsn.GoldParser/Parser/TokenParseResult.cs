// (C) 2010 Ars�ne von Wyss / bsn
namespace bsn.GoldParser.Parser {
	/// <summary>
	/// Result of parsing token.
	/// </summary>
	internal enum TokenParseResult {
		Empty = 0,
		Accept = 1,
		Shift = 2,
		ReduceNormal = 3,
		ReduceEliminated = 4,
		SyntaxError = 5,
		InternalError = 6
	}
}