using DF.Models.Configuration;
using DF.Models.Results;
using System.Collections.Generic;

namespace DF.Models.Results
{
    /// <summary>
    /// Constructs and deconstructs a list of comopnents and the design pattern it belongs to
    /// </summary>
    public class PatternComponentCollection
    {
        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        public Dictionary<Component, List<ComponentMatch>> Components { get; set; }

        /// <summary>
        /// Gets or sets the design pattern.
        /// </summary>
        public DesignPattern DesignPattern { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternComponentCollection"/> class.
        /// </summary>
        /// <param name="designPattern">The design pattern.</param>
        public PatternComponentCollection(DesignPattern designPattern)
        {
            this.DesignPattern = designPattern;
            this.Components = new Dictionary<Component, List<ComponentMatch>>();
        }

        /// <summary>
        /// Deconstructs the specified design pattern.
        /// </summary>
        /// <param name="designPattern">The design pattern.</param>
        /// <param name="components">The components.</param>
        public void Deconstruct(out DesignPattern designPattern, out Dictionary<Component, List<ComponentMatch>> components)
        {
            designPattern = this.DesignPattern;
            components = this.Components;
        }
    }
}
