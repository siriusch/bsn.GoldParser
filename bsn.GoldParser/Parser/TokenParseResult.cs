// $Archive: /Shared/Library2/Sirius2.GoldParser/TokenParseResult.cs $
// $Revision: 1 $
// $UTCDate: 2009-02-26 12:23:53Z $
// $Author: vonwyssa $
// 
// (C) Sirius Technologies AG, Basel. - $NoKeywords:  $
namespace bsn.GoldParser {
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