using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic.StrictCtorParameterMatching
{
    [TestFixture]
    class ParameterMatching
    {
        [Test]
        public void UnmatchedParametersPass()
        {
            var grammar = CompiledGrammarTest.LoadTestGrammar();
            var actions = new SemanticTypeActions<MockToken>(grammar);
            actions.Initialize(true);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnmatchedParametersFail()
        {
            var grammar = CompiledGrammarTest.LoadTestGrammar();
            var actions = new SemanticTypeActions<MockToken>(grammar);
            actions.Initialize(true, true);
        }
    }
}
