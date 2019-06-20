using System.Collections.Generic;

namespace DF.Models.Configuration
{
    /// <summary>
    /// The abstract representation of a design pattern, consisting of one or several components.
    /// </summary>
    public class DesignPattern
    {
        /// <summary>
        /// Gets or sets the components of the design pattern
        /// </summary>
        public List<Component> Components { get; set; }

        /// <summary>
        /// Gets or sets the name of the design pattern
        /// </summary>
        public string Name { get; set; }
    }
}
