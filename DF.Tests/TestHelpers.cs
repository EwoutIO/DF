using DF.Core.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DF.Tests
{
    /// <summary>
    /// This class provides testdata and helper methods for the various unit tests.
    /// </summary>
    public class TestHelpers
    {
        /// <summary>
        /// Provide a INamedTypeSymbol based on the testclass supplied as a string. <seealso cref="Constants"/> for the data.
        /// </summary>
        /// <param name="testClass">The test class in a string format.</param>
        /// <returns>A named type symbol based on the provided data</returns>
        public INamedTypeSymbol GetMockClass(string testClass)
        {
            var symbolProvider = new SymbolProvider();

            var wrappedTestClass = this.WrapInNamespace(testClass);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(wrappedTestClass);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var compilation = CSharpCompilation.Create("TestCompilation")
                              .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                              .AddSyntaxTrees(tree);

            var symbols = symbolProvider.GetNamedTypeSymbols(new List<Compilation>() { compilation });

            return symbols.Single();
        }

        /// <summary>
        /// Provide a ISymbol based on the testmember supplied as a string. <seealso cref="Constants"/> for the data.
        /// </summary>
        /// <param name="testMember">The test member.</param>
        /// <param name="testMemberName">Name of the test member.</param>
        /// <returns></returns>
        public ISymbol GetMockMember(string testMember, string testMemberName)
        {
            var wrappedTestMember = this.WrapInNamespace(WrapInClass(testMember));

            var @class = GetMockClass(wrappedTestMember);

            return @class.GetMembers(testMemberName).Single();
        }

        /// <summary>
        /// Wraps the testmethod in a class.
        /// </summary>
        /// <param name="testMember">The test member.</param>
        /// <returns>A member in a mocked class as a string</returns>
        public string WrapInClass(string testMember) => $"class TestClass {{ {testMember} }}";

        /// <summary>
        /// Wraps the testclass in a namespace.
        /// </summary>
        /// <param name="testClass">The test class.</param>
        /// <returns>A namespaced testclass string</returns>
        public string WrapInNamespace(string testClass) => $"using System; using System.Collections.Generic; using System.Text; namespace TestNamespace {{ {testClass} }}";
    }
}
