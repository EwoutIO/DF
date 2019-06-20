using DF.Models.Configuration;
using DF.Models.Results;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF.Finder.SymbolVisitors
{
    /// <summary>
    /// Class responsible for visitingn the members of the namespace, the namedtype symbols.
    /// </summary>
    /// <seealso cref="DesignFinder.Finder.BaseSymbolVisitor" />
    public class NamedTypeSymbolVisitor : BaseSymbolVisitor
    {
        /// <summary>
        /// The member symbol lock, prevent multiple components updating the resultlist at once
        /// </summary>
        private readonly object memberSymbolLock = new object();

        /// <summary>
        /// Visits the specified symbol, matches all candidates with the symbol. Returns whether a match was found, and which candidates matched.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there was a match, and the matched componenttypes. Empty list if there are no matches</returns>
        public (bool match, IEnumerable<ComponentMatch> componentMatches) Visit(INamedTypeSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var componentMatches = new List<ComponentMatch>();
            var memberMatches = new List<MemberMatch>();

            candidates = this.MatchSymbolNamedTypeKind(symbol, candidates);
            candidates = this.MatchAccessModifier(symbol, candidates);
            candidates = this.MatchModifier(symbol, candidates);
            candidates = this.MatchInheritance(symbol, candidates);

            // Return early when no components matched. No need to scan all members.
            if (!candidates.Any())
            {
                return (false, componentMatches);
            }

            // match childmembers, if the parent matches

            (candidates, memberMatches) = this.MatchMembers(symbol, candidates);

            var symbolPath = symbol.DeclaringSyntaxReferences[0]?.SyntaxTree?.FilePath ?? "Filepath undefined.";
            foreach (var candidate in candidates)
            {
                var match = new ComponentMatch()
                {
                    Component = (Component)candidate,
                    SymbolInformation = new SymbolInformation() { Symbol = symbol, SymbolName = this.GetFullName(symbol), SymbolPath = symbolPath },
                    MemberMatches = memberMatches.Where(c => c.Member.Parent == candidate)
                };

                componentMatches.Add(match);
            }

            return (candidates.Any(), componentMatches);
        }

        /// <summary>
        /// Gets the full name of the symbol, including the namespace
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <returns>The full name</returns>
        private string GetFullName(ISymbol symbol) => $"{symbol.ContainingNamespace}.{symbol.Name}";

        /// <summary>
        /// Check if the inheritance matches the defined component heritance
        /// </summary>
        /// <param name="allInterfaces"></param>
        /// <returns>[true] if there are still candidates left</returns>
        private IEnumerable<IBaseConfigModel> MatchInheritance(INamedTypeSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var symbolInterfaces = symbol.AllInterfaces;

            var matches = new List<IBaseConfigModel>();

            foreach (Component candidate in candidates)
            {
                var inheritanceMatches = true;

                foreach (var inheritance in candidate.Inheritances)
                {
                    foreach (var @interface in symbolInterfaces)
                    {
                        if (!inheritance.Custom)
                        {
                            inheritanceMatches &= this.MatchNamedTypeKind(@interface, candidates).Any();
                            inheritanceMatches &= @interface.Name == inheritance.Name;
                        }
                        else
                        {
                            // Not able to determine the correct type at this time
                            continue;
                        }
                    }
                }

                matches.Add(candidate);
            }

            return matches;
        }

        /// <summary>
        /// Match the members of the symbol to the candidates' members
        /// </summary>
        /// <param name="symbol">The current symbol</param>
        /// <returns>[true] if there are candidates left</returns>
        private (IEnumerable<IBaseConfigModel> candidates, List<MemberMatch> memberMatches) MatchMembers(INamedTypeSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var memberVisitor = new MemberSymbolVisitor();
            var symbolMembers = symbol.GetMembers();

            var memberMatches = new List<MemberMatch>();
            var componentMembers = candidates.SelectMany(c => ((Component)c).Members).Cast<IBaseConfigModel>().ToList();

            Parallel.ForEach(symbolMembers, (symbolMember) =>
            {
                if (symbolMember != null)
                {
                    var symbolMemberMatches = memberVisitor.Visit(symbolMember, componentMembers);

                    if (symbolMemberMatches.Any())
                    {
                        lock (memberSymbolLock)
                        {
                            memberMatches.AddRange(symbolMemberMatches);
                        }
                    }
                }
            });

            var candidateMatches = this.RemoveUnusedMethodComponents(componentMembers, memberMatches, candidates);
            return (candidateMatches, memberMatches);
        }

        /// <summary>
        /// Matches the kind of the named type.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        private IEnumerable<IBaseConfigModel> MatchNamedTypeKind(INamedTypeSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            var matches = new List<IBaseConfigModel>();

            foreach (Component candidate in candidates)
            {
                if (candidate.ComponentType == TypeKind.Unknown || symbol.TypeKind.Equals(candidate.ComponentType))
                {
                    matches.Add(candidate);
                }
            }

            return matches;
        }

        /// <summary>
        /// Matches the namedtype symbol kind with the componenttype of the candidates
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>True if there are any candidates left</returns>
        private IEnumerable<IBaseConfigModel> MatchSymbolNamedTypeKind(INamedTypeSymbol symbol, IEnumerable<IBaseConfigModel> candidates)
        {
            return this.MatchNamedTypeKind(symbol, candidates);
        }

        /// <summary>
        /// Remove the candidate components from the list if their members do not match the membervalues of the symbol.
        /// </summary>
        /// <param name="componentMembers">The component members.</param>
        /// <param name="memberMatches">The member matches.</param>
        /// <returns>True if there are any candidates left</returns>
        private IEnumerable<IBaseConfigModel> RemoveUnusedMethodComponents(IEnumerable<IBaseConfigModel> componentMembers, IEnumerable<MemberMatch> memberMatches, IEnumerable<IBaseConfigModel> componentCandidates)
        {
            var candidatesToRemove = new List<IBaseConfigModel>();

            foreach (var componentMember in componentMembers)
            {
                var matches = memberMatches.Where(m => m.Member == componentMember);

                if ((!matches.Any() && componentMember.Occurrence.MinimumOccurrence != "0")
                    || (matches.Any() && componentMember.Occurrence.MinimumOccurrence == "0")
                    || (matches.Any() && matches.Count() > 1 && componentMember.Occurrence.MaximumOccurrence != "N")
                    || (matches.Any() && matches.Count() <= 1 && componentMember.Occurrence.MinimumOccurrence == "N"))
                {
                    candidatesToRemove.Add(componentMember.Parent);
                }
            }

            return componentCandidates.Except(candidatesToRemove).ToList();
        }
    }
}
