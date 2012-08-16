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
        private CompiledGrammar _grammar;

        [TestFixtureSetUp]
        protected void SetUp()
        {
            _grammar = CompiledGrammarTest.LoadTestGrammar();
        }       

        [Test]
        public void BaseTest()
        {
            var actionsA = new SemanticTypeActions<MockTokenBase>(_grammar);
            Assert.Throws<InvalidOperationException>(() => actionsA.Initialize(true, true));
            
            var actionsB = new SemanticTypeActions<MockTokenBase>(_grammar);
            Assert.DoesNotThrow(() => actionsB.Initialize(true,false));

            //default should be false
            var actionsC = new SemanticTypeActions<MockTokenBase>(_grammar);
            Assert.DoesNotThrow(() => actionsC.Initialize(true));
        }

        [Test]
        public void ExplicitCheckTest()
        {
            var actionsA = new SemanticTypeActions<MockTokenBaseExplicitChecks>(_grammar);
            Assert.Throws<InvalidOperationException>(() => actionsA.Initialize(true, true));

            var actionsB = new SemanticTypeActions<MockTokenBaseExplicitChecks>(_grammar);
            Assert.Throws<InvalidOperationException>(() => actionsB.Initialize(true, false));            
        }

        [Test]
        public void ExplicitNoCheckTest()
        {
            var actionsA = new SemanticTypeActions<MockTokenBaseExplicitNoChecks>(_grammar);
            Assert.DoesNotThrow(() => actionsA.Initialize(true, true));

            var actionsB = new SemanticTypeActions<MockTokenBaseExplicitNoChecks>(_grammar);
            Assert.DoesNotThrow(() => actionsB.Initialize(true, false));
        }

    }
}
