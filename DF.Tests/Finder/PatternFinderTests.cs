using DF.Finder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF.Tests.Finder
{
    /// <summary>
    /// Class responsible for testing <see cref="PatternFinder"/> class
    /// </summary>
    [TestClass]
    public class PatternFinderTests
    {
        /// <summary>
        /// Debug test, change the solutionpath and configurationpath to a local solution
        /// </summary>
        /// <returns>A task, so the testrunner does not stop early</returns>
        [TestMethod]
        public async Task DebugPatternFinderTest()
        {
            // Arrange
            var solutionPath = Path.GetFullPath(@"..\Solution.sln");
            var configurationItemPath = Path.GetFullPath(@"..\ConfigurationItem.xml");

            var patternFinder = new PatternFinder();
            await patternFinder.Initialize(new List<string> { configurationItemPath }, new List<string>() { solutionPath }, true);

            // Act
            var results = patternFinder.FindPatterns();

            // Assert
            Assert.IsNotNull(results);
        }
    }
}
