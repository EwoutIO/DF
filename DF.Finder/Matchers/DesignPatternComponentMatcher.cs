using DF.Finder.Graph;
using DF.Models;
using DF.Models.Configuration;
using DF.Models.Results;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DF.Finder.Matchers
{
    /// <summary>
    /// Class responsible for the 'second pass', after getting all component matches, checking if the design patterns are complete. Only return the complete patterns
    /// </summary>
    public class DesignPatternComponentMatcher
    {
        /// <summary>
        /// Stores the list of possible implementations.
        /// </summary>
        private List<Dictionary<Component, List<ComponentMatch>>> possibleImplementations;

        /// <summary>
        /// Matches the design pattern with the components.
        /// </summary>
        /// <param name="scMatches">The symbol component matches.</param>
        /// <returns>The complete patterns</returns>
        public List<FoundDesignPattern> MatchDesignPatternComponents(Dictionary<Component, List<ComponentMatch>> scMatches)
        {
            var collections = this.GetPatternComponentsCollections(scMatches);

            return this.GetValidPatternCollections(collections);
        }

        /// <summary>
        /// Simplify the data structure by 'unpacking' the dictionary of componentmatches to symbolinformation
        /// </summary>
        /// <param name="possibleImplementation">The possible implementation with componentmatches</param>
        /// <returns>The implementation with symbol information</returns>
        private Dictionary<Component, List<SymbolInformation>> CastDictionary(Dictionary<Component, List<ComponentMatch>> possibleImplementation)
        {
            var implementationDictionary = new Dictionary<Component, List<SymbolInformation>>();

            foreach (var key in possibleImplementation.Keys)
            {
                implementationDictionary.Add(key, possibleImplementation[key].Select(c => c.SymbolInformation).ToList());
            }

            return implementationDictionary;
        }

        /// <summary>
        /// Gets the pattern components collections.
        /// </summary>
        /// <param name="scMatches">The symbol component matches.</param>
        /// <returns>The component matches grouped by design pattern.</returns>
        private List<PatternComponentCollection> GetPatternComponentsCollections(Dictionary<Component, List<ComponentMatch>> scMatches)
        {
            var collections = new List<PatternComponentCollection>();

            foreach (var scMatch in scMatches)
            {
                var pattern = scMatch.Key.DesignPattern;
                var collection = collections.SingleOrDefault(c => c.DesignPattern == pattern);

                if (collection == null)
                {
                    collection = new PatternComponentCollection(pattern);
                    collections.Add(collection);
                }

                if (!collection.Components.ContainsKey(scMatch.Key))
                {
                    collection.Components.Add(scMatch.Key, new List<ComponentMatch>());
                }

                // Defensive: avoid possible matches that did not correctly add to the list.
                var cleanedValue = scMatch.Value.Where(c => c != null);
                collection.Components[scMatch.Key].AddRange(cleanedValue);
            }

            return collections;
        }

        /// <summary>
        /// Gets the pattern implementations.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="componentMatches">The component matches.</param>
        /// <returns>A list of pattern implementations that need to be checked on completeness</returns>
        private List<Dictionary<Component, List<ComponentMatch>>> GetPatternImplementations(PatternGraph graph, Dictionary<Component, List<ComponentMatch>> componentMatches)
        {
            this.possibleImplementations = new List<Dictionary<Component, List<ComponentMatch>>>();
            var singularComponentNames = graph.GetSingularComponentNames();

            if (singularComponentNames.Count == 0)
            {
                // Todo. Not supported for v1. - Removed exception, just return null.
                return null;
            }

            var startGraphComponent = graph.GetGraphComponent(singularComponentNames[0]);
            var startComponentMatches = componentMatches[startGraphComponent.Component];

            foreach (var startComponentMatch in startComponentMatches)
            {
                this.possibleImplementations.Add(new Dictionary<Component, List<ComponentMatch>>()
                {
                    { startGraphComponent.Component, new List<ComponentMatch>() { startComponentMatch } }
                });
            }

            var foundComponents = new List<GraphComponent>() { startGraphComponent };

            return this.MapImplementations(graph, foundComponents, componentMatches);
        }

        /// <summary>
        /// Gets a pattern result if it is valid, based on the design pattern configuration
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The valid design pattern</returns>
        private FoundDesignPattern GetPatternResult(PatternComponentCollection collection)
        {
            (var pattern, var componentMatches) = collection;

            // If a pattern does not contain the specified amount of components as matches, no result.
            if (pattern.Components.Count != componentMatches.Keys.Count)
            {
                return null;
            }

            // if a pattern only has a single component, all matches belong to the same match.
            if (pattern.Components.Count == 1 && componentMatches.Keys.Count == 1)
            {
                return this.MapSingleComponentPattern(pattern, componentMatches);
            }

            return this.MapMultipleComponentPattern(pattern, componentMatches);
        }

        /// <summary>
        /// Gets a list of symbolinformation-models that belong to the component in a design pattern
        /// </summary>
        /// <param name="implementedPattern">The implemented pattern.</param>
        /// <param name="component">The component.</param>
        /// <returns>A list of symbol information models that belong to the component</returns>
        private List<SymbolInformation> GetSymbolInformation(Dictionary<Component, List<ComponentMatch>> implementedPattern, Component component)
        {
            if (implementedPattern.ContainsKey(component))
            {
                return implementedPattern[component].Select(p => p.SymbolInformation).ToList();
            }

            return new List<SymbolInformation>();
        }

        /// <summary>
        /// Gets symbols that have implementations for multiple components in the same pattern implementation
        /// </summary>
        /// <param name="implementationCandidate">The implementation candidate.</param>
        /// <returns>The list of symbols with their components</returns>
        private Dictionary<ISymbol, List<Component>> GetSymbolMultipleComponentMatches(Dictionary<Component, List<ComponentMatch>> implementationCandidate)
        {
            var symbolComponentMatches = new Dictionary<ISymbol, List<Component>>();
            var componentMatches = implementationCandidate.SelectMany(i => i.Value);

            foreach (var componentMatch in componentMatches)
            {
                var symbol = componentMatch.SymbolInformation.Symbol;

                if (!symbolComponentMatches.ContainsKey(symbol))
                {
                    symbolComponentMatches.Add(componentMatch.SymbolInformation.Symbol, new List<Component>());
                }

                if (!symbolComponentMatches[symbol].Contains(componentMatch.Component))
                {
                    symbolComponentMatches[symbol].Add(componentMatch.Component);
                }
            }

            return symbolComponentMatches.Where(c => c.Value.Count > 1).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Gets the valid pattern collections.
        /// </summary>
        /// <param name="collections">The collections.</param>
        /// <returns>The valid collections</returns>
        private List<FoundDesignPattern> GetValidPatternCollections(List<PatternComponentCollection> collections)
        {
            var foundPatterns = new List<FoundDesignPattern>();

            foreach (var collection in collections)
            {
                var patternResult = this.GetPatternResult(collection);

                if (patternResult != null)
                {
                    foundPatterns.Add(patternResult);
                }
            }

            return foundPatterns;
        }

        /// <summary>
        /// Maps the actual implementations of the designpattern.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="foundComponents">The found components.</param>
        /// <param name="componentMatches">The component matches.</param>
        /// <returns>A list of possible matches, these implementations might be incomplete.</returns>
        private List<Dictionary<Component, List<ComponentMatch>>> MapImplementations(PatternGraph graph, List<GraphComponent> foundComponents, Dictionary<Component, List<ComponentMatch>> componentMatches)
        {
            var mappedImplementations = new List<Dictionary<Component, List<ComponentMatch>>>();

            var remainingMatches = componentMatches.Where(c => foundComponents.Any(fc => fc.Component != c.Key)).ToDictionary(x => x.Key, x => x.Value);
            var noMatchPreviousRound = true;

            while (remainingMatches.Count > 0)
            {
                var matchFound = false;

                foreach (var remainingMatch in remainingMatches)
                {
                    var matchedComponent = this.MatchRemainingComponent(graph, remainingMatch, foundComponents);

                    if (matchedComponent != null)
                    {
                        foundComponents.Add(matchedComponent);
                        matchFound = true;
                    }
                }

                if (matchFound)
                {
                    foreach (var foundComponent in foundComponents)
                    {
                        if (remainingMatches.ContainsKey(foundComponent.Component))
                        {
                            remainingMatches.Remove(foundComponent.Component);
                        }
                    }
                }
                else if (!matchFound && noMatchPreviousRound)
                {
                    // Pattern not updated, no matches to be found. Stop process.
                    remainingMatches.Clear();
                }
                else
                {
                    noMatchPreviousRound = true;
                }
            }

            foreach (var possibleImplementation in this.possibleImplementations)
            {
                if (this.MatchComponentCount(graph.DesignPattern, possibleImplementation))
                {
                    mappedImplementations.Add(possibleImplementation);
                }
            }

            return mappedImplementations;
        }

        /// <summary>
        /// Maps the multiple component pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="componentMatches">The component matches.</param>
        /// <returns>The founddesignpatterns model, with the pattern and a list of implementations</returns>
        private FoundDesignPattern MapMultipleComponentPattern(DesignPattern pattern, Dictionary<Component, List<ComponentMatch>> componentMatches)
        {
            var foundPattern = new FoundDesignPattern()
            {
                DesignPattern = pattern,
                Implementations = new List<Dictionary<Component, List<SymbolInformation>>>()
            };

            var graph = new PatternGraph(pattern);
            var implementationCandidates = this.GetPatternImplementations(graph, componentMatches);

            foreach (var implementationCandidate in implementationCandidates)
            {
                var cleanedImplementation = this.RemoveDuplicateComponentMatches(implementationCandidate);

                if (this.MatchComponentCount(pattern, cleanedImplementation))
                {
                    var checkedImplementation = this.MatchImplementationComponentTypes(graph, cleanedImplementation);

                    if (this.MatchComponentCount(pattern, checkedImplementation) && this.ValidComponentOccurrence(checkedImplementation))
                    {
                        foundPattern.Implementations.Add(this.CastDictionary(checkedImplementation));
                    }
                }
            }

            if (foundPattern.Implementations.Count > 0)
            {
                return foundPattern;
            }

            return null;
        }

        /// <summary>
        /// Maps the single component pattern, to a list of implemented patterns.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="componentMatches">The component matches.</param>
        /// <returns>The founddesignpatterns model, with the pattern and a list of implementations</returns>
        private FoundDesignPattern MapSingleComponentPattern(DesignPattern pattern, Dictionary<Component, List<ComponentMatch>> componentMatches)
        {
            var foundPattern = new FoundDesignPattern()
            {
                DesignPattern = pattern,
                Implementations = new List<Dictionary<Component, List<SymbolInformation>>>()
            };

            foreach (var matches in componentMatches.Values.SelectMany(v => v))
            {
                foundPattern.Implementations.Add(
                    new Dictionary<Component, List<SymbolInformation>>()
                    {
                        {
                            pattern.Components.Single(),
                            new List<SymbolInformation>() { matches.SymbolInformation }
                        }
                    }
                );
            }

            return foundPattern;
        }

        /// <summary>
        /// Matches the component count with the design pattern component count.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="implementationCandidate">The implementation candidate.</param>
        /// <returns>true if the amount of components matches the amount of components in the design pattern</returns>
        private bool MatchComponentCount(DesignPattern pattern, Dictionary<Component, List<ComponentMatch>> implementationCandidate) => implementationCandidate.Keys.Count == pattern.Components.Count;

        /// <summary>
        /// Matches the component types.
        /// </summary>
        /// <param name="childComponent">The child component.</param>
        /// <param name="parentComponent">The parent component.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="componentMatches">The component matches.</param>
        /// <param name="currentComponentIsChild">if set to <c>true</c> [the current (unknown) component is the child].</param>
        private void MatchComponentTypes(GraphComponent childComponent, GraphComponent parentComponent, Reference reference, List<ComponentMatch> componentMatches, bool currentComponentIsChild)
        {
            // reference contains the link to the other component
            foreach (var componentMatch in componentMatches)
            {
                foreach (var implementedPattern in this.possibleImplementations)
                {
                    // if current is child, get types of parent; if current is parent, get types of child
                    var knownComponent = currentComponentIsChild ? parentComponent.Component : childComponent.Component;
                    var knownComponentTypes = this.GetSymbolInformation(implementedPattern, knownComponent);

                    bool typeMatch = currentComponentIsChild ? this.MatchUnknownChildWithKnownParent(componentMatch, knownComponentTypes, reference) : this.MatchKnownChildWithUnknownParent(knownComponentTypes, componentMatch, reference);

                    if (typeMatch)
                    {
                        if (!implementedPattern.ContainsKey(componentMatch.Component))
                        {
                            implementedPattern.Add(componentMatch.Component, new List<ComponentMatch>());
                        }

                        if (!implementedPattern[componentMatch.Component].Contains(componentMatch))
                        {
                            implementedPattern[componentMatch.Component].Add(componentMatch);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Matches the component match types, with the implementations.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="possibleImplementation">The possible implementation.</param>
        private Dictionary<Component, List<ComponentMatch>> MatchImplementationComponentTypes(PatternGraph graph, Dictionary<Component, List<ComponentMatch>> possibleImplementation)
        {
            var componentsToRemove = new List<Component>();

            foreach (var componentToCheck in possibleImplementation)
            {
                var componentToCheckMatch = componentToCheck.Value[0];
                var currentComponent = componentToCheck.Key;
                var currentGraphComponent = graph.GetGraphComponent(currentComponent.Name);

                var otherComponents = possibleImplementation.Where(i => i.Key != currentComponent);
                var otherGraphComponents = graph.GetGraphComponents(otherComponents.Select(c => c.Key.Name));

                var currentComponentReferences = currentGraphComponent.References;
                var currentComponentReferencedBy = otherGraphComponents.Where(c => c.References.Any(r => r.ReferencedComponent == currentGraphComponent));

                var typeMatch = true;

                foreach (var currentComponentReference in currentComponentReferences)
                {
                    var knownParentTypes = this.GetSymbolInformation(possibleImplementation, currentComponentReference.ReferencedComponent.Component);
                    typeMatch &= this.MatchUnknownChildWithKnownParent(componentToCheckMatch, knownParentTypes, currentComponentReference);
                }

                foreach (var referencedBy in currentComponentReferencedBy)
                {
                    var knownChildTypes = this.GetSymbolInformation(possibleImplementation, referencedBy.Component);
                    var reference = referencedBy.References.Single(r => r.ReferencedComponent == currentGraphComponent);
                    typeMatch &= this.MatchKnownChildWithUnknownParent(knownChildTypes, componentToCheckMatch, reference);
                }

                // if there is no match, remove the first hit from the pattern. The pattern is incomplete.
                if (!typeMatch)
                {
                    componentsToRemove.Add(currentGraphComponent.Component);
                }
            }

            foreach (var componentToRemove in componentsToRemove)
            {
                possibleImplementation.Remove(componentToRemove);
            }

            return possibleImplementation;
        }

        /// <summary>
        /// Matches the inheritance.
        /// </summary>
        /// <param name="childSymbol">The child symbol.</param>
        /// <param name="parentSymbol">The parent symbol.</param>
        /// <returns>True if the child symbol implements the parent symbol</returns>
        private bool MatchInheritance(INamedTypeSymbol childSymbol, INamedTypeSymbol parentSymbol) => childSymbol.AllInterfaces.Contains(parentSymbol);

        /// <summary>
        /// The parent is unknown, so the known component types are checked to see if they implement/inherit the given parent symbol.
        /// </summary>
        /// <param name="knownComponentTypes">The known component types.</param>
        /// <param name="componentMatch">The component match.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>True if the given component is inherited by one of the known types.</returns>
        private bool MatchKnownChildWithUnknownParent(List<SymbolInformation> knownComponentTypes, ComponentMatch componentMatch, Reference reference)
        {
            var componentMatchSymbol = (INamedTypeSymbol)componentMatch.SymbolInformation.Symbol;

            if (reference.Inheritance.IsReference)
            {
                var inheritanceMatch = knownComponentTypes.Any(s => this.MatchInheritance((INamedTypeSymbol)s.Symbol, componentMatchSymbol));
                return reference.Inheritance.Occurrence != ComponentOccurrence.Prohibited ? inheritanceMatch : !inheritanceMatch;
            }
            else if (reference.Member.IsReference)
            {
                var memberMatch = knownComponentTypes.Any(s => this.MatchMember((INamedTypeSymbol)s.Symbol, componentMatchSymbol));
                return reference.Member.Occurrence != ComponentOccurrence.Prohibited ? memberMatch : !memberMatch;
            }
            else if (reference.Parameter.IsReference)
            {
                var parameterMatch = knownComponentTypes.Any(s => this.MatchParameter((INamedTypeSymbol)s.Symbol, componentMatchSymbol));
                return reference.Parameter.Occurrence != ComponentOccurrence.Prohibited ? parameterMatch : !parameterMatch;
            }

            return false;
        }

        /// <summary>
        /// Matches the member.
        /// </summary>
        /// <param name="childSymbol">The child symbol.</param>
        /// <param name="parentSymbol">The parent symbol.</param>
        /// <returns>True if the child symbol contains a member with a type/returntype of the parentsymbol</returns>
        private bool MatchMember(INamedTypeSymbol childSymbol, INamedTypeSymbol parentSymbol) => childSymbol.GetMembers().Any(m => SymbolMatcher.MatchDataType(m, parentSymbol));

        /// <summary>
        /// Matches the parameter.
        /// </summary>
        /// <param name="childSymbol">The child symbol.</param>
        /// <param name="parentSymbol">The parent symbol.</param>
        /// <returns>True if the child contains a member with a paramter that is of the type of the parentsymbol</returns>
        private bool MatchParameter(INamedTypeSymbol childSymbol, INamedTypeSymbol parentSymbol)
        {
            var members = childSymbol.GetMembers().Where(m => m is IMethodSymbol).Cast<IMethodSymbol>();
            var parameters = members.SelectMany(m => m.Parameters);

            return parameters.Any(p => SymbolMatcher.MatchDataType(p, parentSymbol));
        }

        /// <summary>
        /// Matches the remaining component.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="remainingMatch">The remaining match.</param>
        /// <param name="foundComponents">The found components.</param>
        /// <returns>The component if it has been matched, null otherwise.</returns>
        private GraphComponent MatchRemainingComponent(PatternGraph graph, KeyValuePair<Component, List<ComponentMatch>> remainingMatch, List<GraphComponent> foundComponents)
        {
            var componentMatchFound = false;

            // check if the component inherits any found components
            var remainingMatchGraphComponent = graph.GetGraphComponent(remainingMatch.Key.Name);
            var foundComponentReferences = remainingMatchGraphComponent.References.Where(r => foundComponents.Contains(r.ReferencedComponent));

            if (foundComponentReferences.Any())
            {
                var componentRef = foundComponentReferences.First();

                this.MatchComponentTypes(remainingMatchGraphComponent, componentRef.ReferencedComponent, componentRef, remainingMatch.Value, true);
                componentMatchFound = true;
            }
            else
            {
                // otherwise check if the component is inherited by any found components
                var referencedByFoundComponents = foundComponents.Where(c => c.References.Any(r => r.ReferencedComponent == remainingMatchGraphComponent));

                if (referencedByFoundComponents.Any())
                {
                    var refByComponent = referencedByFoundComponents.First();
                    var refByComponentReference = refByComponent.References.Single(r => r.ReferencedComponent == remainingMatchGraphComponent);

                    this.MatchComponentTypes(refByComponent, remainingMatchGraphComponent, refByComponentReference, remainingMatch.Value, false);
                    componentMatchFound = true;
                }
            }

            if (componentMatchFound)
            {
                return remainingMatchGraphComponent;
            }

            return null;
        }

        // match inheritances of child to known types
        /// <summary>
        /// The child is unknown, so the type/component inheritances are checked for known components and matched to the unknown child.
        /// </summary>
        /// <param name="componentMatch">The component match.</param>
        /// <param name="knownComponentTypes">The known component types.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>True if the given component inherits one of the known types</returns>
        private bool MatchUnknownChildWithKnownParent(ComponentMatch componentMatch, List<SymbolInformation> knownComponentTypes, Reference reference)
        {
            var componentMatchSymbol = (INamedTypeSymbol)componentMatch.SymbolInformation.Symbol;

            var match = true;
            if (reference.Inheritance.IsReference)
            {
                var inheritanceMatch = knownComponentTypes.Any(s => this.MatchInheritance(componentMatchSymbol, (INamedTypeSymbol)s.Symbol));
                match &= reference.Inheritance.Occurrence != ComponentOccurrence.Prohibited ? inheritanceMatch : !inheritanceMatch;
            }

            if (reference.Member.IsReference)
            {
                var memberMatch = knownComponentTypes.Any(s => this.MatchMember(componentMatchSymbol, (INamedTypeSymbol)s.Symbol));
                match &= reference.Member.Occurrence != ComponentOccurrence.Prohibited ? memberMatch : !memberMatch;
            }

            if (reference.Parameter.IsReference)
            {
                var parameterMatch = knownComponentTypes.Any(s => this.MatchParameter(componentMatchSymbol, (INamedTypeSymbol)s.Symbol));
                match &= reference.Parameter.Occurrence != ComponentOccurrence.Prohibited ? parameterMatch : !parameterMatch;
            }

            return match;
        }

        /// <summary>
        /// Removes the duplicate component matches. This can be done after matching all components.
        /// </summary>
        /// <param name="implementationCandidate">The implementation candidate.</param>
        /// <returns>The updated/cleaned list.</returns>
        private Dictionary<Component, List<ComponentMatch>> RemoveDuplicateComponentMatches(Dictionary<Component, List<ComponentMatch>> implementationCandidate)
        {
            var multipleComponentsSymbols = this.GetSymbolMultipleComponentMatches(implementationCandidate);

            foreach (var multipleComponentsSymbol in multipleComponentsSymbols)
            {
                var multipleMatches = multipleComponentsSymbol.Value.Where(c => implementationCandidate[c].Count > 1);
                var singleMatch = multipleComponentsSymbol.Value.Where(c => implementationCandidate[c].Count == 1);

                if (singleMatch.Count() > 1)
                {
                    // If there are multiple matches for a component with only one symbol per component, the pattern is incomplete. Remove the components.
                    foreach (var comp in singleMatch)
                    {
                        implementationCandidate.Remove(comp);
                    }
                }
                else if (singleMatch.Count() == 1)
                {
                    // If there is one component with this symbol as match, remove it from all components that contain both this symbol and others.
                    foreach (var comp in multipleMatches)
                    {
                        this.RemoveSymbolComponentMatch(implementationCandidate, multipleComponentsSymbol.Key, comp);
                    }
                }
                else
                {
                    // Remove the symbol from all but one components if there are multiple matches for all components.
                    foreach (var comp in multipleMatches.Skip(1))
                    {
                        this.RemoveSymbolComponentMatch(implementationCandidate, multipleComponentsSymbol.Key, comp);
                    }
                }
            }

            return implementationCandidate;
        }

        /// <summary>
        /// Removes the symbol component match.
        /// </summary>
        /// <param name="implementationCandidate">The implementation candidate.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="component">The component.</param>
        private void RemoveSymbolComponentMatch(Dictionary<Component, List<ComponentMatch>> implementationCandidate, ISymbol symbol, Component component)
        {
            var componentMatch = implementationCandidate[component].Single(c => c.SymbolInformation.Symbol == symbol);
            implementationCandidate[component].Remove(componentMatch);
        }

        /// <summary>
        /// Validates the component occurrence.
        /// </summary>
        /// <param name="implementationCandidate">The implementation candidate.</param>
        /// <returns>true if the component implementation count is equal to the pattern component occurrence count</returns>
        private bool ValidComponentOccurrence(Dictionary<Component, List<ComponentMatch>> implementationCandidate)
        {
            var correctComponentCount = true;

            foreach (var component in implementationCandidate)
            {
                var occurrence = component.Key.Occurrence;
                var count = component.Value.Count;

                switch (occurrence.MinimumOccurrence)
                {
                    case Constants.One:
                        correctComponentCount &= count >= 1;
                        break;

                    case Constants.N:
                        correctComponentCount &= count > 1;
                        break;

                    default:
                        break;
                }

                switch (occurrence.MaximumOccurrence)
                {
                    case Constants.Zero:
                        correctComponentCount &= count == 0;
                        break;

                    case Constants.One:
                        correctComponentCount &= count <= 1;
                        break;

                    default:
                        break;
                }
            }

            return correctComponentCount;
        }
    }
}
