// (C) 2010 Arsène von Wyss / bsn
using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public interface ITokenizer {
		CompiledGrammar Grammar {
			get;
		}

		ParseMessage NextToken(out TextToken token);
	}
}