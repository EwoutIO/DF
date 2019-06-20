using DF.Models;
using DF.Models.Configuration;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Linq;

namespace DF.Finder.Matchers
{
    /// <summary>
    /// Provides static matchers, to compare symbolfields with a specific value.
    /// </summary>
    public static class SymbolMatcher
    {
        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        /// <param name="typeSymbol">The type symbol.</param>
        /// <returns></returns>
        public static ITypeSymbol GetCollectionType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                typeSymbol = arrayTypeSymbol.ElementType;
            }

            if (TypeSymbolIsCollection(typeSymbol))
            {
                // We currently do not support Dictionaries or (named) tuples as datatypes.
                if (((INamedTypeSymbol)typeSymbol).TypeArguments.Length > 1)
                {
                    return null;
                }

                typeSymbol = ((INamedTypeSymbol)typeSymbol).TypeArguments.Single();
            }

            return typeSymbol;
        }

        /// <summary>
        /// Returns true if whether the symboltype is a collection and the "isCollection" parameter match.
        /// </summary>
        /// <param name="symbol">The symbol to check</param>
        /// <param name="isCollection">Whether the config item type is a collection</param>
        /// <returns>True if the type of the symbol and the type of the config item match</returns>
        public static bool MatchCollection(ISymbol symbol, bool isCollection)
        {
            var typeSymbol = GetTypeSymbol(symbol);
            if (typeSymbol == null) return false;
            return isCollection == (TypeSymbolIsArray(typeSymbol) || TypeSymbolIsCollection(typeSymbol));
        }

        /// <summary>
        /// Matches the datatype of the symbol with the candidate.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="dataType">The datatype.</param>
        /// <returns>True if the values match</returns>
        public static bool MatchDataType(ISymbol symbol, string dataType)
        {
            if (string.IsNullOrEmpty(dataType)) return true;

            var typeSymbol = GetTypeSymbol(symbol);
            return typeSymbol.Name == dataType;
        }

        /// <summary>
        /// Matches the datatype of the symbol to the type of the referenced symbol
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="referencedSymbol">The referenced symbol.</param>
        /// <returns>true if it is a match</returns>
        public static bool MatchDataType(ISymbol symbol, ISymbol referencedSymbol)
        {
            var typeSymbol = GetTypeSymbol(symbol);

            if (typeSymbol == null) return false;

            if (TypeSymbolIsArray(typeSymbol) || TypeSymbolIsCollection(typeSymbol))
            {
                typeSymbol = GetCollectionType(typeSymbol);
            }

            return typeSymbol == referencedSymbol;
        }

        /// <summary>
        /// Match a symbol's parameters with a memberparameter
        /// </summary>
        /// <param name="symbol">The symbol to check</param>
        /// <param name="memberParameter">The parameter of the member</param>
        /// <param name="componentName">The name of the component, to match with potential custom names</param>
        /// <returns>True if any of the parameters match with the memberparameter</returns>
        public static bool MatchMethodSymbolParameter(IParameterSymbol symbol, Parameter memberParameter, string componentName)
        {
            var match = true;

            if (memberParameter.DataType.Custom && memberParameter.DataType.TypeName == componentName)
            {
                match &= MatchDataType(symbol, symbol.ContainingSymbol);
            }
            else if (!memberParameter.DataType.Custom)
            {
                match &= MatchDataType(symbol, memberParameter.DataType.TypeName);
            }

            match &= MatchCollection(symbol, memberParameter.DataType.Collection);

            return match;
        }

        /// <summary>
        /// Match the modifier of the symbol with the given modifier.
        /// </summary>
        /// <param name="symbol">The current symbol</param>
        /// <param name="modifier">The modifier</param>
        /// <returns>True if the modifier matches</returns>
        public static bool MatchModifier(ISymbol symbol, Modifier modifier)
        {
            switch (modifier)
            {
                case Modifier.Abstract: return symbol.IsAbstract;
                case Modifier.Override: return symbol.IsOverride;
                case Modifier.Sealed: return symbol.IsSealed;
                case Modifier.Virtual: return symbol.IsVirtual;
                case Modifier.Static: return symbol.IsStatic;
                default: return true;
            }
        }

        /// <summary>
        /// Matches the kind of the symbol with the membertype of the candidate.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the kind and the membertype match</returns>
        public static bool MatchSymbolKind(ISymbol symbol, MemberType memberType)
        {
            if (memberType == MemberType.Any) return true;

            if (symbol is IFieldSymbol fieldSymbol) return MatchFieldSymbol(fieldSymbol, memberType);
            else if (symbol is IMethodSymbol methodSymbol) return MatchMethodSymbol(methodSymbol, memberType);
            else if (symbol is INamedTypeSymbol namedTypeSymbol) return MatchNamedTypeSymbol(namedTypeSymbol, memberType);
            else if (symbol is IPropertySymbol propertySymbol) return MatchPropertySymbol(propertySymbol, memberType);
            else if (symbol is IEventSymbol) return MatchEventSymbol(memberType);

            return false;
        }

        /// <summary>
        /// Checks if the type symbol is an array
        /// </summary>
        /// <param name="typeSymbol">The type symbol</param>
        /// <returns>True if it is an array</returns>
        public static bool TypeSymbolIsArray(ITypeSymbol typeSymbol) => typeSymbol.TypeKind.Equals(TypeKind.Array);

        /// <summary>
        /// Checks if the type symbol is a collection
        /// </summary>
        /// <param name="typeSymbol">The type symbol</param>
        /// <returns>True if it is a collection</returns>
        public static bool TypeSymbolIsCollection(ITypeSymbol typeSymbol) => typeSymbol.Name.Equals(nameof(ICollection)) || typeSymbol.AllInterfaces.Any(i => i.Name.Equals(nameof(ICollection)));

        /// <summary>
        /// Gets the typesymbol from the given symbol, if it exists.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <returns>The type symbol</returns>
        private static ITypeSymbol GetTypeSymbol(ISymbol symbol)
        {
            ITypeSymbol typeSymbol;
            if (symbol is IFieldSymbol fieldSymbol) typeSymbol = fieldSymbol.Type;
            else if (symbol is IMethodSymbol methodSymbol) typeSymbol = methodSymbol.ReturnType;
            else if (symbol is INamedTypeSymbol namedTypeSymbol) typeSymbol = namedTypeSymbol;
            else if (symbol is IPropertySymbol propertySymbol) typeSymbol = propertySymbol.Type;
            else if (symbol is IEventSymbol eventSymbol) typeSymbol = eventSymbol.Type;
            else if (symbol is IParameterSymbol parameterSymbol) typeSymbol = parameterSymbol.Type;
            else return null;

            return typeSymbol;
        }

        /// <summary>
        /// Matches the event symbol membertype
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the types match</returns>
        private static bool MatchEventSymbol(MemberType memberType)
        {
            switch (memberType)
            {
                case MemberType.Event:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Matches the field symbol membertype
        /// </summary>
        /// <param name="fieldSymbol">The field symbol.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the values match</returns>
        private static bool MatchFieldSymbol(IFieldSymbol fieldSymbol, MemberType memberType)
        {
            switch (memberType)
            {
                case MemberType.Constant:
                    return fieldSymbol.IsConst;

                case MemberType.Field:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Matches the method symbol membertype
        /// </summary>
        /// <param name="methodSymbol">The method symbol.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the values match</returns>
        private static bool MatchMethodSymbol(IMethodSymbol methodSymbol, MemberType memberType)
        {
            switch (memberType)
            {
                case MemberType.Constructor:
                    return methodSymbol.MethodKind == MethodKind.Constructor;

                case MemberType.Finalizer:
                    return methodSymbol.MethodKind == MethodKind.Destructor;

                case MemberType.Method:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// Matches the named type symbol membertype
        /// </summary>
        /// <param name="namedTypeSymbol">The named type symbol.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the values match</returns>
        private static bool MatchNamedTypeSymbol(INamedTypeSymbol namedTypeSymbol, MemberType memberType)
        {
            switch (memberType)
            {
                case MemberType.Enum:
                    return namedTypeSymbol.TypeKind == TypeKind.Enum;

                case MemberType.Class:
                    return namedTypeSymbol.TypeKind == TypeKind.Class;

                case MemberType.Interface:
                    return namedTypeSymbol.TypeKind == TypeKind.Interface;

                case MemberType.Struct:
                    return namedTypeSymbol.TypeKind == TypeKind.Struct;

                case MemberType.Delegate:
                    return namedTypeSymbol.TypeKind == TypeKind.Delegate;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Matches the property symbol member type.
        /// </summary>
        /// <param name="propertySymbol">The property symbol.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns>True if the values match</returns>
        private static bool MatchPropertySymbol(IPropertySymbol propertySymbol, MemberType memberType)
        {
            switch (memberType)
            {
                case MemberType.Indexer:
                    return propertySymbol.IsIndexer;

                case MemberType.Property:
                    return true;

                default: return false;
            }
        }
    }
}