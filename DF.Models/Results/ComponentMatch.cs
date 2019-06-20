using DF.Models.Configuration;
using System.Collections.Generic;

namespace DF.Models.Results
{
    /// <summary>
    /// The model to contain the component and matching members and symbol
    /// </summary>
    public class ComponentMatch
    {
        /// <summary>
        /// Gets or sets the component that is matched
        /// </summary>
        public Component Component { get; set; }

        /// <summary>
        /// Gets or sets a list of matches of the members
        /// </summary>
        public IEnumerable<MemberMatch> MemberMatches { get; set; }

        /// <summary>
        /// Gets or sets the symbol information model for the component
        /// </summary>
        public SymbolInformation SymbolInformation { get; set; }
    }
}
