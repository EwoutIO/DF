using DF.Core.IO;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF.Core.Symbols
{
    /// <summary>
    /// This class is responsible for providing the symbols needed by the designfinder.
    /// </summary>
    public class SymbolProvider
    {
        private readonly WorkspaceLoader workspaceLoader;

        public SymbolProvider()
        {
            this.workspaceLoader = new WorkspaceLoader();
        }

        /// <summary>
        /// Get the list of named type symbols from multiple compilations
        /// </summary>
        /// <param name="compilations">The compilations</param>
        /// <returns>The combined list of named type symbols</returns>
        /// <remarks>
        /// Be careful when loading projects from different solutions, as there might be an overlap in namespace/classname-combinations.
        /// </remarks>
        public List<INamedTypeSymbol> GetNamedTypeSymbols(List<Compilation> compilations)
        {
            var symbols = new List<INamedTypeSymbol>();

            foreach (var compilation in compilations)
            {
                var compilationSymbols = this.GetNamedTypeSymbols(compilation);
                symbols.AddRange(compilationSymbols);
            }

            return symbols;
        }

        /// <summary>
        /// Retrieve the list of named type symbols from the global namespace of the compilation
        /// </summary>
        /// <param name="compilation">The compilation of a project</param>
        /// <returns>a list of named type symbols</returns>
        public List<INamedTypeSymbol> GetNamedTypeSymbols(Compilation compilation)
        {
            var visitor = new GetAllSymbolsVisitor();
            return visitor.GetAllSymbols(compilation.Assembly.GlobalNamespace);
        }

        /// <summary>
        /// Get a list of named type symbols from a single project file
        /// </summary>
        /// <param name="path">The path of the project</param>
        /// <returns>A list of named type symbols</returns>
        public async Task<List<INamedTypeSymbol>> GetNamedTypeSymbolsFromProjectFile(string path)
        {
            var compilation = await this.workspaceLoader.GetCompilationFromProjectPath(path);

            return this.GetNamedTypeSymbols(compilation);
        }

        /// <summary>
        /// Get the list of named type symbols from multiple project paths
        /// </summary>
        /// <param name="paths">The paths of multiple projects</param>
        /// <returns>The combined list of named type symbols</returns>
        /// <remarks>
        /// Be careful when loading projects from different solutions, as there might be an overlap in namespace/classname-combinations.
        /// </remarks>
        public async Task<List<INamedTypeSymbol>> GetNamedTypeSymbolsFromProjectFiles(List<string> paths)
        {
            var compilations = await this.workspaceLoader.GetCompilationsFromProjectPaths(paths);

            return this.GetNamedTypeSymbols(compilations);
        }

        /// <summary>
        /// Get the list of named type symbols from a solution path
        /// </summary>
        /// <param name="path">The path of the solution</param>
        /// <returns>a list of named type symbols</returns>
        public async Task<List<INamedTypeSymbol>> GetNamedTypeSymbolsFromSolutionFile(string path)
        {
            var compilations = await this.workspaceLoader.GetCompilationsFromSolutionPath(path);

            return this.GetNamedTypeSymbols(compilations);
        }
    }
}
