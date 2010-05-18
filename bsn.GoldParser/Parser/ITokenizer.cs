// (C) 2010 Ars�ne von Wyss / bsn
using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Parser {
	public interface ITokenizer<T> {
		CompiledGrammar Grammar {
			get;
		}

		ParseMessage NextToken(out T token);
	}
}