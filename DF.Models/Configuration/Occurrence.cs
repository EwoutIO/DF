namespace DF.Models.Configuration
{
    /// <summary>
    /// The occurrence defines how many times a certain object is allowed in the design pattern. Allowed values include 0, 1 and N
    /// </summary>
    /// <remarks>
    /// As 0 and 1 are numbers, they cannot be defined as such in the enumerator. Check if we want to lock this down somehow.
    /// </remarks>
    public class Occurrence
    {
        /// <summary>
        /// Gets or sets the maximum occurrence
        /// </summary>
        public string MaximumOccurrence { get; set; }

        /// <summary>
        /// Gets or sets the minimum occurrence
        /// </summary>
        public string MinimumOccurrence { get; set; }
    }
}
