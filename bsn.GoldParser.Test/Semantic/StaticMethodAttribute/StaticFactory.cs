using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
{
    class StaticFactory
    {
        [Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>")]
        [Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>")]
        public static MockTokenDerived Expression(MockTokenDerived a, MockTokenBase b, MockTokenBase c)
        {
            return new MockTokenDerived(a, b, c);
        }

        [Rule(@"<Expression> ::= <Mult Exp>")]
        public static MockTokenDerived Expression(MockTokenBase a)
        {
            return new MockTokenDerived(a);
        }

        [Rule(@"<Value> ::= '(' <Expression> ')'")]
        public static MockTokenDerived Value(MockTokenBase a, MockTokenDerived b, MockTokenBase c)
        {
            return new MockTokenDerived(b,a,c);
        }

        [Rule(@"<Empty> ::= ")]
        public static MockTokenBase Empty()
        {
            return new MockTokenBase();
        }

        [Rule(@"<Negate Exp> ::= '-' <Value>",false)]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
        public static MockTokenBase Basic(MockTokenBase a, MockTokenBase b, MockTokenBase c)
        {
            return new MockTokenBase(a, b, c);
        }
    }
}
