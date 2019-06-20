using System.Collections.Generic;

namespace DF.Models.Results
{
    /// <summary>
    /// The inheritance names model
    /// </summary>
    public class InheritanceNames
    {
        /// <summary>
        /// Gets or sets the inherited component names
        /// </summary>
        public IEnumerable<(string componentName, ComponentOccurrence occurrence)> inheritances { get; set; }

        /// <summary>
        /// Gets or sets the member component names
        /// </summary>
        public IEnumerable<(string componentName, ComponentOccurrence occurrence)> members { get; set; }

        /// <summary>
        /// Gets or sets the parameter component names
        /// </summary>
        public IEnumerable<(string componentName, ComponentOccurrence occurrence)> parameters { get; set; }
    }
}
