namespace DF.Models.Configuration
{
    /// <summary>
    /// The parameter is a part of the signature of a method or constructor, this defines the type and whether it is mandatory
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Gets or sets the data type of the parameter
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the occurrence
        /// </summary>
        public Occurrence Occurrence { get; set; }
    }
}
