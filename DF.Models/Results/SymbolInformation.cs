using Microsoft.CodeAnalysis;

namespace DF.Models.Results
{
    /// <summary>
    /// Model to contain the symbol information needed during the matching process.
    /// </summary>
    public class SymbolInformation
    {
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public ISymbol Symbol { get; set; }

        /// <summary>
        /// Gets or sets the name of the symbol.
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Gets or sets the symbol path.
        /// </summary>
        public string SymbolPath { get; set; }
    }
}
