using DF.Models.Configuration;
using Microsoft.CodeAnalysis;

namespace DF.Models.Results
{
    /// <summary>
    /// Model that matches the parameter with a symbol. Note: use for custom parameters to match later.
    /// </summary>
    public class ParameterMatch
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        public Parameter Parameter { get; set; }

        /// <summary>
        /// Gets or sets the parametersymbol.
        /// </summary>
        public ISymbol ParameterSymbol { get; set; }
    }
}
