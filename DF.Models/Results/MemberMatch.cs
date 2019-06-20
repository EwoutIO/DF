using DF.Models.Configuration;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DF.Models.Results
{
    /// <summary>
    /// The member match, for internal use only, not used for delivering data.
    /// </summary>
    public class MemberMatch
    {
        /// <summary>
        /// The found member
        /// </summary>
        public Member Member { get; set; }

        /// <summary>
        /// Gets or sets the ParameterMatches
        /// </summary>
        public List<ParameterMatch> ParameterMatches { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public ISymbol Symbol { get; set; }
    }
}
