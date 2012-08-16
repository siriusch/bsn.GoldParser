using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
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
    class DynamicMockTokenBase : SemanticToken
    {
        public DynamicMockTokenBase(){}
    }
}
