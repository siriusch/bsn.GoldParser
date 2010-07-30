using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<Value> ::= '(' <Expression> ')'", "<Expression>", SemanticTokenType = typeof(TestToken))]