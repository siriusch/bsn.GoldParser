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
    class MockTokenBaseExplicitNoChecks : SemanticToken
    {
        public MockTokenBaseExplicitNoChecks(string text) { }

        [Rule(@"<Empty> ::= ", false)]
        [Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
        [Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
        [Rule(@"<Negate Exp> ::= '-' <Value>", false)]
        [Rule(@"<Value> ::= '(' <Expression> ')'")]
        public MockTokenBaseExplicitNoChecks(MockTokenBaseExplicitNoChecks a, MockTokenBaseExplicitNoChecks b, MockTokenBaseExplicitNoChecks c) { }
    }
}
