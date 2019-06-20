using DF.Models.Configuration;
using System.Collections.Generic;

namespace DF.Models
{
    public class ConfigurationModel
    {
        /// <summary>
        /// The components of all defined design patterns
        /// </summary>
        public List<Component> Components { get; set; }

        /// <summary>
        /// The configuration model design patterns, the full pattern list.
        /// </summary>
        public List<DesignPattern> DesignPatterns { get; set; }
    }
}
