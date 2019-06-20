using DF.Finder.SymbolVisitors;
using DF.Models;
using DF.Models.Configuration;
using DF.Models.Results;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF.Finder.Matchers
{
    /// <summary>
    /// The symbol component matcher, matches symbols to defined components.
    /// </summary>
    public class SymbolComponentMatcher
    {
        /// <summary>
        /// The component match lock, prevent multiple components updating the resultlist at once
        /// </summary>
        private readonly object componentMatchLock = new object();

        /// <summary>
        /// The configuration model to use, will be set during the Initialize step
        /// </summary>
        private readonly ConfigurationModel configurationModel;

        /// <summary>
        /// The namespace members to analyze, will be set during the Initialize step
        /// </summary>
        private readonly List<INamedTypeSymbol> namespaceMembers;

        /// <summary>
        /// The component matches
        /// </summary>
        private readonly Dictionary<Component, List<ComponentMatch>> componentMatches;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolComponentMatcher"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="namespaceMembers">The namespace members.</param>
        public SymbolComponentMatcher(ConfigurationModel configuration, List<INamedTypeSymbol> namespaceMembers)
        {
            this.configurationModel = configuration;
            this.namespaceMembers = namespaceMembers;
            this.componentMatches = new Dictionary<Component, List<ComponentMatch>>();
        }

        /// <summary>
        /// Analyze the workspace
        /// </summary>
        /// <returns>The results of the analysis</returns>
        public Dictionary<Component, List<ComponentMatch>> AnalyzeSymbols()
        {
            var classVisitor = new NamedTypeSymbolVisitor();

            Parallel.ForEach(this.namespaceMembers, (symbol) =>
            {
                if (symbol != null)
                {
                    var componentCandidates = configurationModel.Components.Cast<IBaseConfigModel>().ToList();
                    var (match, componentMatches) = classVisitor.Visit(symbol, componentCandidates);

                    if (match)
                    {
                        foreach (var componentMatch in componentMatches)
                        {
                            this.AddComponentMatch(componentMatch);
                        }
                    }
                }
            });

            return this.componentMatches;
        }

        /// <summary>
        /// Update the dictionary of component matches with the found component match
        /// </summary>
        /// <param name="patternName">The name of the pattern to update</param>
        /// <param name="component">component</param>
        private void AddComponentMatch(ComponentMatch componentMatch)
        {
            if (!this.componentMatches.ContainsKey(componentMatch.Component))
            {
                // Utilize locking to enable parallel running of the scanning
                lock (componentMatchLock)
                {
                    if (!this.componentMatches.ContainsKey(componentMatch.Component))
                    {
                        this.componentMatches.Add(componentMatch.Component, new List<ComponentMatch>());
                    }
                }
            }

            this.componentMatches[componentMatch.Component].Add(componentMatch);
        }
    }
}
