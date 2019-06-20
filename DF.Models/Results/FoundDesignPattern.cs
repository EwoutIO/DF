using DF.Models.Configuration;
using System.Collections.Generic;

namespace DF.Models.Results
{
    /// <summary>
    /// Model that represents a design pattern and all concrete matches. Contains a list of dictionaries of components with their symbols.
    /// </summary>
    public class FoundDesignPattern
    {
        /// <summary>
        /// Gets or sets the configuration design pattern.
        /// </summary>
        public DesignPattern DesignPattern { get; set; }

        /// <summary>
        /// Gets or sets the implementations. Every dictionary represents one implementation, containing entries for each component of the pattern
        /// with a list of classes that implement that part of the pattern.
        /// </summary>
        public List<Dictionary<Component, List<SymbolInformation>>> Implementations { get; set; }
    }
}
