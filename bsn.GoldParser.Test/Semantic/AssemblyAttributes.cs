using System;
using System.Linq;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<Value> ::= '(' <Expression> ')'", "<Expression>")]
