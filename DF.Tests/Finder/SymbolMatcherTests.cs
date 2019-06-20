using DF.Finder.Matchers;
using DF.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF.Tests.Finder
{
    /// <summary>
    /// Class responsible for testing the <see cref="SymbolMatcher"/> class
    /// </summary>
    [TestClass]
    public class SymbolMatcherTests
    {
        /// <summary>
        /// Tests the matching of datatypes against the different types of ISymbol
        /// </summary>
        [TestMethod]
        public void MatchDataTypeTest()
        {
            // Arrange
            var helpers = new TestHelpers();

            var fieldMember = helpers.GetMockMember(Constants.PrivateFieldMember, "TestMemberName");
            var methodMember = helpers.GetMockMember(Constants.PublicMethodMember, "TestMemberName");
            var namedTypeMember = helpers.GetMockMember(Constants.PublicNamedTypeMember, "TestMemberName");
            var propertyMember = helpers.GetMockMember(Constants.PublicPropertyMember, "TestMemberName");
            var eventMember = helpers.GetMockMember(Constants.PublicEventMember, "TestMemberName");

            // Act
            var fieldMatch = SymbolMatcher.MatchDataType(fieldMember, "String");
            var methodMatch = SymbolMatcher.MatchDataType(methodMember, "Void");
            var namedTypeMatch = SymbolMatcher.MatchDataType(namedTypeMember, "TestMemberName");
            var propertyMatch = SymbolMatcher.MatchDataType(propertyMember, "String");
            var eventMatch = SymbolMatcher.MatchDataType(eventMember, "DelegateMember");

            var anyMatch = SymbolMatcher.MatchDataType(fieldMember, string.Empty);

            var noFieldMatch = SymbolMatcher.MatchDataType(fieldMember, "wrong");
            var noMethodMatch = SymbolMatcher.MatchDataType(methodMember, "wrong");
            var noNamedTypeMatch = SymbolMatcher.MatchDataType(namedTypeMember, "wrong");
            var noPropertyMatch = SymbolMatcher.MatchDataType(propertyMember, "wrong");
            var noEventMatch = SymbolMatcher.MatchDataType(eventMember, "wrong");

            // Assert
            Assert.IsTrue(fieldMatch);
            Assert.IsTrue(methodMatch);
            Assert.IsTrue(namedTypeMatch);
            Assert.IsTrue(propertyMatch);
            Assert.IsTrue(eventMatch);

            Assert.IsTrue(anyMatch);

            Assert.IsFalse(noFieldMatch);
            Assert.IsFalse(noMethodMatch);
            Assert.IsFalse(noNamedTypeMatch);
            Assert.IsFalse(noPropertyMatch);
            Assert.IsFalse(noEventMatch);
        }

        /// <summary>
        /// Tests the matching of modifiers against the different types of ISymbol
        /// </summary>
        [TestMethod]
        public void MatchModifierTest()
        {
            // Arrange
            var helpers = new TestHelpers();
            var abstractMember = helpers.GetMockMember(Constants.AbstractMember, "TestMemberName");
            var overrideMember = helpers.GetMockMember(Constants.OverrideMember, "TestMemberName");
            var sealedMember = helpers.GetMockMember(Constants.SealedMember, "TestMemberName");
            var staticMember = helpers.GetMockMember(Constants.StaticMember, "TestMemberName");
            var virtualMember = helpers.GetMockMember(Constants.VirtualMember, "TestMemberName");

            // Act
            var abstractMatch = SymbolMatcher.MatchModifier(abstractMember, Models.Modifier.Abstract);
            var overrideMatch = SymbolMatcher.MatchModifier(overrideMember, Models.Modifier.Override);
            var sealedMatch = SymbolMatcher.MatchModifier(sealedMember, Models.Modifier.Sealed);
            var staticMatch = SymbolMatcher.MatchModifier(staticMember, Models.Modifier.Static);
            var virtualMatch = SymbolMatcher.MatchModifier(virtualMember, Models.Modifier.Virtual);

            var anyMatch = SymbolMatcher.MatchModifier(abstractMember, Models.Modifier.Any);

            var noAbstractMatch = SymbolMatcher.MatchModifier(abstractMember, Models.Modifier.Virtual);
            var noOverrideMatch = SymbolMatcher.MatchModifier(overrideMember, Models.Modifier.Abstract);
            var noSealedMatch = SymbolMatcher.MatchModifier(sealedMember, Models.Modifier.Abstract);
            var noStaticMatch = SymbolMatcher.MatchModifier(staticMember, Models.Modifier.Abstract);
            var noVirtualMatch = SymbolMatcher.MatchModifier(virtualMember, Models.Modifier.Abstract);

            // Assert
            Assert.IsTrue(abstractMatch);
            Assert.IsTrue(overrideMatch);
            Assert.IsTrue(sealedMatch);
            Assert.IsTrue(staticMatch);
            Assert.IsTrue(virtualMatch);

            Assert.IsTrue(anyMatch);

            Assert.IsFalse(noAbstractMatch);
            Assert.IsFalse(noOverrideMatch);
            Assert.IsFalse(noSealedMatch);
            Assert.IsFalse(noStaticMatch);
            Assert.IsFalse(noVirtualMatch);
        }

        /// <summary>
        /// Tests the matching of symbolkinds against the different types of ISymbol
        /// </summary>
        [TestMethod]
        public void MatchSymbolKindTest()
        {
            // Arrange
            var helpers = new TestHelpers();

            var fieldMember = helpers.GetMockMember(Constants.PrivateFieldMember, "TestMemberName");
            var methodMember = helpers.GetMockMember(Constants.PublicMethodMember, "TestMemberName");
            var namedTypeMember = helpers.GetMockMember(Constants.PublicNamedTypeMember, "TestMemberName");
            var propertyMember = helpers.GetMockMember(Constants.PublicPropertyMember, "TestMemberName");
            var eventMember = helpers.GetMockMember(Constants.PublicEventMember, "TestMemberName");

            // Act
            var fieldMatch = SymbolMatcher.MatchSymbolKind(fieldMember, MemberType.Field);
            var methodMatch = SymbolMatcher.MatchSymbolKind(methodMember, MemberType.Method);
            var namedTypeMatch = SymbolMatcher.MatchSymbolKind(namedTypeMember, MemberType.Class);
            var propertyMatch = SymbolMatcher.MatchSymbolKind(propertyMember, MemberType.Property);
            var eventMatch = SymbolMatcher.MatchSymbolKind(eventMember, MemberType.Event);

            var anyMatch = SymbolMatcher.MatchSymbolKind(fieldMember, MemberType.Any);

            var noFieldMatch = SymbolMatcher.MatchSymbolKind(fieldMember, MemberType.Property);
            var noMethodMatch = SymbolMatcher.MatchSymbolKind(methodMember, MemberType.Field);
            var noNamedTypeMatch = SymbolMatcher.MatchSymbolKind(namedTypeMember, MemberType.Field);
            var noPropertyMatch = SymbolMatcher.MatchSymbolKind(propertyMember, MemberType.Field);
            var noEventMatch = SymbolMatcher.MatchSymbolKind(eventMember, MemberType.Field);

            // Assert
            Assert.IsTrue(fieldMatch);
            Assert.IsTrue(methodMatch);
            Assert.IsTrue(namedTypeMatch);
            Assert.IsTrue(propertyMatch);
            Assert.IsTrue(eventMatch);

            Assert.IsTrue(anyMatch);

            Assert.IsFalse(noFieldMatch);
            Assert.IsFalse(noMethodMatch);
            Assert.IsFalse(noNamedTypeMatch);
            Assert.IsFalse(noPropertyMatch);
            Assert.IsFalse(noEventMatch);
        }
    }
}
