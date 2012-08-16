using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StrictCtorParameterMatching
{
    [Terminal("(EOF)")]
    [Terminal("(Error)")]
    [Terminal("(Whitespace)")]
    [Terminal("(Comment End)")]
    [Terminal("(Comment Line)")]
    [Terminal("(Comment Start)")]
    [Terminal("-")]
    [Terminal("(")]
    [Terminal(")")]
    [Terminal("*")]
    [Terminal("/")]
    [Terminal("+")]
    [Terminal("Float")]
    [Terminal("Integer")]
    [Terminal("NULL")]
    [Terminal("String")]
    class MockTokenBaseExplicitChecks : SemanticToken
    {
        public MockTokenBaseExplicitChecks(string text){}

        [Rule(@"<Empty> ::= ",true)]
        [Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
        [Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
        [Rule(@"<Negate Exp> ::= '-' <Value>", true)]
        [Rule(@"<Value> ::= '(' <Expression> ')'")]
        public MockTokenBaseExplicitChecks(MockTokenBase a, MockTokenBase b, MockTokenBase c){ }
    }
}
