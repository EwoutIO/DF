using DF.Finder.SymbolVisitors;
using DF.Models;
using DF.Models.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF.Tests.Finder
{
    /// <summary>
    /// Class responsible for testing the <see cref="MemberSymbolVisitor"/> class
    /// </summary>
    [TestClass]
    public class MemberSymbolVisitorTests
    {
        /// <summary>
        /// Tests if given a correct member, the member is recognized by the algorithm as a match, given the provided design pattern
        /// </summary>
        [TestMethod]
        public void MatchMemberSymbolTest()
        {
            // Arrange
            var helpers = new TestHelpers();
            var pattern = this.GetDesignPattern();
            var member = pattern.Components[0].Members[0];
            member.Parent = pattern.Components[0];

            var symbol = helpers.GetMockMember(Constants.PublicStaticMethodMember, "TestMemberName");
            var visitor = new MemberSymbolVisitor();
            var candidates = pattern.Components[0].Members.Cast<IBaseConfigModel>().ToList();

            // Act
            var results = visitor.Visit(symbol, candidates);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());

            Member resultMember = (Member)results.Single().Member;
            Assert.IsNotNull(resultMember);
            Assert.AreEqual("TestComponent", resultMember.DataType.TypeName);
        }

        /// <summary>
        /// Tests if given an incorrect member, the member is recognized by the algorithm as not a match, given the provided design pattern
        /// </summary>
        [TestMethod]
        public void NoMatchMemberSymbolTest()
        {
            // Arrange
            var helpers = new TestHelpers();
            var pattern = this.GetDesignPattern();
            var member = pattern.Components[0].Members[0];
            member.Parent = pattern.Components[0];

            var symbol = helpers.GetMockMember(Constants.PrivateMethodMember, "TestMemberName");
            var visitor = new MemberSymbolVisitor();
            var candidates = pattern.Components[0].Members.Cast<IBaseConfigModel>().ToList();

            // Act
            var results = visitor.Visit(symbol, candidates);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        /// <summary>
        /// Provides a design pattern with a single member for the tests.
        /// </summary>
        /// <returns>The mock design pattern</returns>
        private DesignPattern GetDesignPattern()
        {
            return new DesignPattern()
            {
                Name = "TestDesignPattern",
                Components = new List<Component>()
                {
                    new Component()
                    {
                        DesignPatternName = "TestDesignpattern",
                        Members = new List<Member>() {
                            new Member()
                            {
                                MemberType = MemberType.Method,
                                AccessModifier = Accessibility.Public,
                                Modifier = Modifier.Static,
                                DataType = new DataType()
                                {
                                    Collection = false,
                                    Custom = true,
                                    TypeName = "TestComponent"
                                },
                                Occurrence = new Occurrence()
                                {
                                    MinimumOccurrence = "1",
                                    MaximumOccurrence = "1"
                                },
                                Parameters = new List<Parameter>()
                            }
                        },
                        ComponentType = TypeKind.Class,
                        AccessModifier = Accessibility.Private,
                        Modifier = Models.Modifier.Static,
                        Name = "TestComponent",
                        Inheritances = new List<Inheritance>(),
                        Occurrence = new Occurrence
                        {
                            MinimumOccurrence = "1",
                            MaximumOccurrence = "1"
                        }
                    }
                }
            };
        }
    }
}
