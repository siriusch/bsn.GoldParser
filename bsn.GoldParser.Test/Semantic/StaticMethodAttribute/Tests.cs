using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using bsn.GoldParser.Grammar;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
{
    [TestFixture]
    class Tests
    {
        private CompiledGrammar _grammar;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _grammar = CompiledGrammarTest.LoadTestGrammar();
        }


        [Test]
        public void Initializes()
        {
            var actions = new SemanticTypeActions<MockTokenBase>(_grammar);
            Assert.DoesNotThrow(actions.Initialize);
        }

        [Test]
        public void GenericInitializes()
        {
            var actions = new SemanticTypeActions<MockGenericTokenBase>(_grammar);
            Assert.DoesNotThrow(actions.Initialize);
        }

        [Test]
        public void GenericParse()
        {
            var actions = new SemanticTypeActions<MockGenericTokenBase>(_grammar);
            actions.Initialize();

            var processor = new SemanticProcessor<MockGenericTokenBase>(new StringReader("-1+2+3*4-8"), actions);
            Assert.AreEqual(ParseMessage.Accept,processor.ParseAll());
            Assert.IsInstanceOf<MockGenericTokenBase>(processor.CurrentToken);
        }

        [Test]
        public void DynamicFactory()
        {
            var actions = new SemanticTypeActions<DynamicMockTokenBase>(_grammar);
            Assert.Throws<InvalidOperationException>(actions.Initialize);

            actions = new SemanticTypeActions<DynamicMockTokenBase>(_grammar);
            try
            {
                actions.Initialize();
            }
            catch (InvalidOperationException exception)
            {
                Assert.True(Regex.IsMatch(exception.Message, "Rule .* is assigned to a non-static method, which is not allowed."));
            }
        }
    }
}
