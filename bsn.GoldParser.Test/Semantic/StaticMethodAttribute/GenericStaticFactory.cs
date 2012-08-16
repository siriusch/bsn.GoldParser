using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
{
    class GenericStaticFactory
    {
        [Rule(@"<Value> ::= '(' <Expression> ')'")]
        [Rule(@"<Negate Exp> ::= '-' <Value>", false)]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '*' <Negate Exp>")]
        [Rule(@"<Mult Exp> ::= <Mult Exp> '/' <Negate Exp>")]
        public static MockGenericTokenBase Create(MockGenericTokenBase a, MockGenericTokenBase b, MockGenericTokenBase c)
        {
            return new MockGenericTokenBase();
        }

        [Rule(@"<Expression> ::= <Expression> '+' <Mult Exp>", typeof(MockGenericTokenBase))]
        [Rule(@"<Expression> ::= <Expression> '-' <Mult Exp>", typeof(MockGenericTokenBase))]
        [Rule(@"<Expression> ::= <Mult Exp>", typeof(MockGenericTokenBase))]
        [Rule(@"<Empty> ::= ", typeof(MockGenericTokenBase))]
        public static T Instance<T>(T a, T b, T c) where T : MockGenericTokenBase, new()
        {
            return new T();
        }
    }
}
