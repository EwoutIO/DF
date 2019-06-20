using DF.Models.Configuration;
using DF.Models.Results;
using System;
using System.Collections.Generic;

namespace DF.CLI.Results
{
    /// <summary>
    /// Format the results with fancy colours and indentation.
    /// </summary>
    public static class ResultFormatter
    {
        /// <summary>
        /// Prints the results.
        /// </summary>
        /// <param name="foundDesignPatterns">The found design patterns.</param>
        public static void PrintResults(List<FoundDesignPattern> foundDesignPatterns)
        {
            foreach (var pattern in foundDesignPatterns)
            {
                PrintPattern(pattern);
            }
        }

        /// <summary>
        /// Prints the given (and found) pattern. (no indentation)
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        private static void PrintPattern(FoundDesignPattern pattern)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"DesignPattern: {pattern.DesignPattern.Name}");
            Console.ResetColor();

            for (int i = 0; i < pattern.Implementations.Count; i++)
            {
                PrintImplementation(i, pattern.Implementations[i]);
            }
        }

        /// <summary>
        /// Prints a list of implementations for the given patterns (two space indentation)
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="patternImplementation">The pattern implementation.</param>
        private static void PrintImplementation(int count, Dictionary<Component, List<SymbolInformation>> patternImplementation)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{GetWhitespace(2)}Implementation {count + 1}:");
            Console.ResetColor();

            foreach (KeyValuePair<Component, List<SymbolInformation>> componentImplementation in patternImplementation)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{GetWhitespace(4)}{componentImplementation.Key.Name}");
                Console.ResetColor();

                PrintComponentSymbolInformationList(componentImplementation.Value);
            }
        }

        /// <summary>
        /// Prints the component symbol information list, based on the given pattern implementation (four space indentation).
        /// </summary>
        /// <param name="symbolInformationList">The symbol information list.</param>
        private static void PrintComponentSymbolInformationList(List<SymbolInformation> symbolInformationList)
        {
            foreach (var symbolInformation in symbolInformationList)
            {
                Console.WriteLine($"{GetWhitespace(6)}{symbolInformation.SymbolName} ({symbolInformation.SymbolPath})");
            }
        }

        /// <summary>
        /// Gets a number of whitespace items depending on the count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>The defined amount of whitespace</returns>
        private static string GetWhitespace(int count) => new string(' ', count);
    }
}
