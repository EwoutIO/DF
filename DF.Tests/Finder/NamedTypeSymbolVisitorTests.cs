using DF.Finder.SymbolVisitors;
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
    /// Class responsible for testing the <see cref="NamedTypeSymbolVisitor"/> class
    /// </summary>
    [TestClass]
    public class NamedTypeSymbolVisitorTests
    {
        /// <summary>
        /// Tests if given a correct named type symbol, the symbol is recognized by the algorithm as a match, given the provided design pattern
        /// </summary>
        [TestMethod]
        public void MatchNamedTypeSymbolTest()
        {
            // Arrange
            var helpers = new TestHelpers();
            var pattern = this.GetDesignPattern();
            var symbol = helpers.GetMockClass(Constants.EmptyPrivateStaticClass);
            var visitor = new NamedTypeSymbolVisitor();

            var candidates = pattern.Components.Cast<IBaseConfigModel>();

            // Act
            var results = visitor.Visit(symbol, candidates);

            // Assert
            Assert.IsNotNull(results);
            Assert.IsTrue(results.match);
            Assert.AreEqual(1, results.componentMatches.Count());

            var componentMatch = results.componentMatches.Single();

            Assert.AreEqual("TestDesignpattern", componentMatch.Component.DesignPatternName);
            Assert.AreEqual("TestComponent", componentMatch.Component.Name);
        }

        /// <summary>
        /// Tests if given an incorrect named type symbol, the symbol is recognized by the algorithm as not a match, given the provided design pattern
        /// </summary>
        [TestMethod]
        public void NoMatchNamedTypeSymbolTest()
        {
            // Arrange
            var helpers = new TestHelpers();
            var pattern = this.GetDesignPattern();
            var symbol = helpers.GetMockClass(Constants.EmptyPublicClass);
            var visitor = new NamedTypeSymbolVisitor();
            var candidates = pattern.Components.Cast<IBaseConfigModel>();

            // Act
            var results = visitor.Visit(symbol, candidates);

            // Assert
            Assert.IsNotNull(results);
            Assert.IsFalse(results.match);
            Assert.AreEqual(0, results.componentMatches.Count());
        }

        /// <summary>
        /// Provides a design pattern with a component without members, as we only want to test the named type functionality
        /// </summary>
        /// <returns>The design pattern</returns>
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
                        Members = new List<Member>(),

                        ComponentType = TypeKind.Class,
                        AccessModifier = Accessibility.Private,
                        Modifier = Models.Modifier.Static,
                        Name = "TestComponent",
                        Inheritances = new List<Inheritance>()
                        {
                            new Inheritance()
                            {
                                ComponentType = TypeKind.Interface,
                                Custom = true,
                                Name = "ImplementedInterface"
                            }
                        },
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
