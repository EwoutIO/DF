using DF.Finder.Matchers;
using DF.Models.Configuration;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DF.Finder.SymbolVisitors
{
    /// <summary>
    /// Baseclass for the visitors, handles all symbol-agnostic logic
    /// </summary>
    public abstract class BaseSymbolVisitor
    {
        /// <summary>
        /// Checks if the custom datatype is equal to the component it belongs to.
        /// </summary>
        /// <param name="dataType">The current datatype</param>
        /// <param name="componentName">The componentname</param>
        /// <returns>True if the datatype is equal to the component</returns>
        protected bool IsCustomAndParent(DataType dataType, string componentName) => dataType.Custom && dataType.TypeName == componentName;

        /// <summary>
        /// Match the access modifier of the symbol with the access modifier of the candidates. Remove the candidates that do not match the modifier.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there are any candidates left.</returns>
        protected IEnumerable<IBaseConfigModel> MatchAccessModifier(ISymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            foreach (var candidate in candidates)
            {
                if (candidate.AccessModifier.Equals(Accessibility.NotApplicable) || symbol.DeclaredAccessibility.Equals(candidate.AccessModifier))
                {
                    matches.Add(candidate);
                }
            }

            return matches;
        }

        /// <summary>
        /// Matches the modifier of the symbol with the access modifier of the candidates. Remove the candidates that do not match the modifier.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there are any candidates left.</returns>
        protected IEnumerable<IBaseConfigModel> MatchModifier(ISymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            foreach (var candidate in candidates)
            {
                if (SymbolMatcher.MatchModifier(symbol, candidate.Modifier))
                {
                    matches.Add(candidate);
                }
            }

            return matches;
        }
    }
}
