using DF.Finder.Matchers;
using DF.Models.Configuration;
using DF.Models.Results;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DF.Finder.SymbolVisitors
{
    /// <summary>
    /// Class responsible for visiting the membersymbols. In this case, these are the namedtype symbol members: the members of the namespacemembers.
    /// </summary>
    /// <seealso cref="DesignFinder.Finder.BaseSymbolVisitor" />
    public class MemberSymbolVisitor : BaseSymbolVisitor
    {
        /// <summary>
        /// Returns a list of members that match the symbol.
        /// </summary>
        /// <param name="symbol">The membersymbol to check</param>
        /// <returns>The list of matched candidates</returns>
        public IEnumerable<MemberMatch> Visit(ISymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            candidates = this.MatchSymbolType(symbol, candidates);
            candidates = base.MatchAccessModifier(symbol, candidates);
            candidates = base.MatchModifier(symbol, candidates);
            candidates = this.MatchDataType(symbol, candidates);

            var parameterMatches = new Dictionary<Member, List<ParameterMatch>>();

            if (!candidates.Any())
            {
                return new List<MemberMatch>();
            }

            if (symbol is IMethodSymbol methodSymbol)
            {
                (candidates, parameterMatches) = this.MatchMethodParameters(methodSymbol, candidates);
            }

            var matches = new List<MemberMatch>();

            foreach (Member match in candidates)
            {
                var memberMatch = new MemberMatch() { Member = match, Symbol = symbol };

                if (parameterMatches.ContainsKey(match))
                {
                    memberMatch.ParameterMatches = parameterMatches[match];
                }

                matches.Add(memberMatch);
            }

            return matches;
        }

        /// <summary>
        /// Matches the datatype of the symbol and the candidates
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there are candidates left</returns>
        private IEnumerable<IBaseConfigModel> MatchDataType(ISymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            foreach (Member member in candidates)
            {
                var typeMatch = true;

                if (member.DataType?.Custom == true && member.DataType.TypeName == ((Component)member.Parent).Name)
                {
                    // returns the parentname, can be checked
                    typeMatch = SymbolMatcher.MatchDataType(symbol, symbol.ContainingSymbol.Name);
                }
                else if (member.DataType?.Custom == false)
                {
                    typeMatch = SymbolMatcher.MatchDataType(symbol, member.DataType.Type.Name);
                }
                else
                {
                    // Not able to determine the correct type at this time
                }

                if (member.DataType != null)
                {
                    typeMatch &= SymbolMatcher.MatchCollection(symbol, member.DataType.Collection);
                }

                if (typeMatch)
                {
                    matches.Add(member);
                }
            }

            return matches;
        }

        /// <summary>
        /// Match the parameters of the methodsymbol with the parameters of the method candidates
        /// </summary>
        /// <param name="symbol">The current method symbol</param>
        /// <param name="candidates">The method candidates</param>
        /// <returns>A list of matching members</returns>
        private (IEnumerable<IBaseConfigModel> matchedCandidates, Dictionary<Member, List<ParameterMatch>> parameterMatches) MatchMethodParameters(IMethodSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            var parameterMatches = new Dictionary<Member, List<ParameterMatch>>();

            foreach (Member candidate in candidates)
            {
                var componentName = ((Component)candidate.Parent).Name;
                var parameterSymbols = symbol.Parameters;

                // If no parameters are defined, match is valid by default.
                if (candidate.Parameters.Count == 0)
                {
                    matches.Add(candidate);
                }
                else
                {
                    var methodParameterMatch = true;

                    foreach (var parameter in candidate.Parameters)
                    {
                        var hasParameterMatch = false;

                        foreach (var parameterSymbol in parameterSymbols)
                        {
                            var currentParameterMatch = SymbolMatcher.MatchMethodSymbolParameter(parameterSymbol, parameter, componentName);
                            hasParameterMatch |= currentParameterMatch;

                            if (currentParameterMatch && base.IsCustomAndParent(parameter.DataType, ((Component)candidate.Parent).Name))
                            {
                                // add custom types to parametermatches, for easier mapping later on.
                                if (!parameterMatches.ContainsKey(candidate))
                                {
                                    parameterMatches.Add(candidate, new List<ParameterMatch>());
                                }

                                parameterMatches[candidate].Add(new ParameterMatch() { Parameter = parameter, ParameterSymbol = parameterSymbol });
                            }
                        }

                        methodParameterMatch &= hasParameterMatch;
                    }

                    if (methodParameterMatch)
                    {
                        matches.Add(candidate);
                    }
                }
            }

            return (matches, parameterMatches);
        }

        /// <summary>
        /// Matches the type of the symbol with the membertype of the candidates
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there are any candidates left</returns>
        private IEnumerable<IBaseConfigModel> MatchSymbolType(ISymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            foreach (Member candidate in candidates)
            {
                if (SymbolMatcher.MatchSymbolKind(symbol, candidate.MemberType))
                {
                    matches.Add(candidate);
                }
            }

            return matches;
        }
    }
}
