using DF.Core.Symbols;
using DF.Finder.Configuration;
using DF.Finder.Matchers;
using DF.Models;
using DF.Models.Results;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF.Finder
{
    public class PatternFinder
    {
        /// <summary>
        /// The configuration model to use, will be set during the Initialize step
        /// </summary>
        private ConfigurationModel configurationModel;

        /// <summary>
        /// The namespace members to analyze, will be set during the Initialize step
        /// </summary>
        private List<INamedTypeSymbol> namespaceMembers;

        /// <summary>
        /// Finds the patterns, by scanning through the solution symbols and matching the components afterwards.
        /// </summary>
        /// <returns>a list of found design patterns with implementations per pattern</returns>
        public List<FoundDesignPattern> FindPatterns()
        {
            var scMatcher = new SymbolComponentMatcher(this.configurationModel, this.namespaceMembers);
            var scMatches = scMatcher.AnalyzeSymbols();

            var dpcMatcher = new DesignPatternComponentMatcher();
            var dpcMatches = dpcMatcher.MatchDesignPatternComponents(scMatches);

            return dpcMatches;
        }

        /// <summary>
        /// Initialize the PatternFinder using the given parameters.
        /// </summary>
        /// <param name="configurationFilePaths">The filepaths to the configuration files</param>
        /// <param name="workspaceFilePaths">The workspace file paths, can be singular</param>
        /// <param name="isSolution">If true, the file is handled as a solution, only used when the workspace file paths contains one value</param>
        /// <param name="includeTestProjects">If true, the testprojects of a solution will be loaded as well</param>
        public async Task Initialize(List<string> configurationFilePaths, List<string> workspaceFilePaths, bool isSolution)
        {
            this.configurationModel = this.GetConfiguration(configurationFilePaths);
            this.namespaceMembers = await this.GetNamespaceMembers(workspaceFilePaths, isSolution);
        }

        /// <summary>
        /// Get the configuration, based on the configuration file paths
        /// </summary>
        /// <param name="configurationFilePaths">The paths to the xml files</param>
        /// <returns>the created configurationmodel</returns>
        private ConfigurationModel GetConfiguration(List<string> configurationFilePaths)
        {
            var configurationProvider = new ConfigurationProvider();
            return configurationProvider.GetConfigurationModel(configurationFilePaths, true);
        }

        /// <summary>
        /// Get the namespace members of the given workspace
        /// </summary>
        /// <param name="workspaceFilePaths">The workspace file paths, can be singular</param>
        /// <param name="isSolution">If true, the file is handled as a solution, only used when the workspace file paths contains one value</param>
        /// <param name="includeTestProjects">If true, the testprojects of a solution will be loaded as well</param>
        /// <returns>A list of named type symbols to analyze</returns>
        private async Task<List<INamedTypeSymbol>> GetNamespaceMembers(List<string> workspaceFilePaths, bool isSolution)
        {
            var symbolProvider = new SymbolProvider();

            if (workspaceFilePaths.Count < 1)
            {
                throw new Exception("No workspace file path given.");
            }

            if (workspaceFilePaths.Count > 1)
            {
                return await symbolProvider.GetNamedTypeSymbolsFromProjectFiles(workspaceFilePaths);
            }

            if (isSolution)
            {
                return await symbolProvider.GetNamedTypeSymbolsFromSolutionFile(workspaceFilePaths[0]);
            }
            else
            {
                return await symbolProvider.GetNamedTypeSymbolsFromProjectFile(workspaceFilePaths[0]);
            }
        }
    }
}
