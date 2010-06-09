using System;
using System.Collections.ObjectModel;
using System.Linq;

using NUnit.Framework;

namespace bsn.GoldParser.Semantic {
	[TestFixture]
	public class TypeUtilityTest: AssertionHelper {
		[Test]
		public void Create() {
			new TypeUtility<SemanticToken>();
		}

		[Test]
		public void GetCommonBaseTypeAncestor() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			Expect(typeUtility.GetCommonBaseType(typeof(TestAdd), typeof(TestSubtract)), EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void GetCommonBaseTypeNoCommon() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			Expect(typeUtility.GetCommonBaseType(typeof(TestValue), typeof(TestSubtract)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetCommonBaseTypeSame() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			Expect(typeUtility.GetCommonBaseType(typeof(TestToken), typeof(TestToken)), EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetEmptyAncestors() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			ReadOnlyCollection<Type> baseTypes = typeUtility.GetBaseTypes(typeof(TestToken));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(0));
		}

		[Test]
		public void GetSeveralAncestors() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			ReadOnlyCollection<Type> baseTypes = typeUtility.GetBaseTypes(typeof(TestAdd));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(2));
			Expect(baseTypes[0], EqualTo(typeof(TestToken)));
			Expect(baseTypes[1], EqualTo(typeof(TestOperation)));
		}

		[Test]
		public void GetSingleAncestor() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			ReadOnlyCollection<Type> baseTypes = typeUtility.GetBaseTypes(typeof(TestOperation));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(1));
			Expect(baseTypes[0], EqualTo(typeof(TestToken)));
		}

		[Test]
		public void GetSymbolType() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			ReadOnlyCollection<Type> baseTypes = typeUtility.GetBaseTypes(typeof(TestAdd));
			Expect(baseTypes, Not.Null);
			Expect(baseTypes.Count, EqualTo(2));
			Expect(baseTypes[0], EqualTo(typeof(TestToken)));
			Expect(baseTypes[1], EqualTo(typeof(TestOperation)));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidType() {
			TypeUtility<TestToken> typeUtility = new TypeUtility<TestToken>();
			typeUtility.GetBaseTypes(typeof(SemanticToken));
		}
	}
}
