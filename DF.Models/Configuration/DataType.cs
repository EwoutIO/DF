using System;

namespace DF.Models.Configuration
{
    /// <summary>
    /// The datatype of the defined object
    /// </summary>
    public class DataType
    {
        /// <summary>
        /// Gets or sets whether this datatype is a collection of the given type
        /// </summary>
        public bool Collection { get; set; }

        /// <summary>
        /// Gets or sets whether this datatype is custom defined in the design pattern configuration file
        /// </summary>
        public bool Custom { get; set; }

        /// <summary>
        /// Gets or sets the actual type of the datatype
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the type name
        /// </summary>
        public string TypeName { get; set; }
    }
}
