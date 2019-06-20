using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF.Core.Symbols
{
    /// <summary>
    /// An extension of the provided SymbolVisitor, this visitor's task is to return a list of symbols for the provided namespace.
    /// </summary>
    public class GetAllSymbolsVisitor : SymbolVisitor
    {
        /// <summary>
        /// The symbols. Because we implement a baseclass, we are limited in how we store the objects we need.
        /// </summary>
        private readonly List<INamedTypeSymbol> symbols = new List<INamedTypeSymbol>();

        /// <summary>
        /// Retrieve a list of all symbols in the given namespace
        /// </summary>
        /// <param name="symbol">The global namespace of the compilation</param>
        /// <returns>A list of named symbols</returns>
        public List<INamedTypeSymbol> GetAllSymbols(INamespaceSymbol symbol)
        {
            this.VisitNamespace(symbol);
            return this.symbols;
        }

        /// <summary>
        /// Visit the found symbols, add them to a list to be retrieved later.
        /// </summary>
        /// <param name="symbol">A symbol that represents a found member from the namespace</param>
        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            this.symbols.Add(symbol);
        }

        /// <summary>
        /// Visit the namespace members in parallel.
        /// </summary>
        /// <param name="symbol">The global namespace of the compilation</param>
        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
        }
    }
}
