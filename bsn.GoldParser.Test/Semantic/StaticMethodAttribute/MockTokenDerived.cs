using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.GoldParser.Semantic.StaticMethodAttribute
{
    class MockTokenDerived : MockTokenBase
    {
        public MockTokenDerived(MockTokenDerived token, params MockTokenBase[] tokens)
        {            
        }

        public MockTokenDerived(MockTokenBase token){}
    }
}
