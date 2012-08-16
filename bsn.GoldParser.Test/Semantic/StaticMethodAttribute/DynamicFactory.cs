using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
{
    class DynamicFactory
    {
        [Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
        [Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
        [Rule(@"<Expression> ::= <Mult Exp>")]
        [Rule(@"<Value> ::= '(' <Expression> ')'")]
        [Rule(@"<Empty> ::= ")]
        [Rule(@"<Negate Exp> ::= '-' <Value>", false)]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
        public DynamicMockTokenBase Get(DynamicMockTokenBase a, DynamicMockTokenBase b, DynamicMockTokenBase c)
        {
            return null;
        }
    }
}
