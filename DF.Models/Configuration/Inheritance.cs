using Microsoft.CodeAnalysis;
using System;

namespace DF.Models.Configuration
{
    /// <summary>
    /// This class defines an inheritance of a component, with a named link to the parent or implemented interface
    /// </summary>
    public class Inheritance
    {
        /// <summary>
        /// Gets or sets the type of the inherited class or interface
        /// </summary>
        public TypeKind ComponentType { get; set; }

        /// <summary>
        /// Gets or sets whether this is a custom type, defined in the configuration file
        /// </summary>
        public bool Custom { get; set; }

        /// <summary>
        /// Gets or sets the name of the type of the inherited object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the actual type of the inherited object
        /// </summary>
        /// <remarks>
        /// For custom inheritance, this means the link can only be set near the end of the process, when all classes are found.
        /// </remarks>
        public Type Type { get; set; }
    }
}
