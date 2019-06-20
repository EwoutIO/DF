using DF.Models;

namespace DF.Finder.Graph
{
    /// <summary>
    /// The Edge of the Graph, containing reference information
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is an inheritance reference.
        /// </summary>
        public (bool IsReference, ComponentOccurrence Occurrence) Inheritance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a member reference.
        /// </summary>
        public (bool IsReference, ComponentOccurrence Occurrence) Member { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a parameter reference.
        /// </summary>
        public (bool IsReference, ComponentOccurrence Occurrence) Parameter { get; set; }

        /// <summary>
        /// Gets or sets the referenced component.
        /// </summary>
        public GraphComponent ReferencedComponent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        /// <param name="referencedComponent">The referenced component.</param>
        /// <param name="referenceType">Type of the reference.</param>
        public Reference(GraphComponent referencedComponent, ReferenceTypes referenceType, ComponentOccurrence componentOccurrence)
        {
            this.ReferencedComponent = referencedComponent;
            this.SetReferenceType(referenceType, componentOccurrence);
            referencedComponent.IsReferenced = true;
        }

        /// <summary>
        /// Sets the type of the reference.
        /// </summary>
        /// <param name="referenceType">Type of the reference.</param>
        public void SetReferenceType(ReferenceTypes referenceType, ComponentOccurrence componentOccurrence)
        {
            switch (referenceType)
            {
                case ReferenceTypes.Inheritance:
                    this.Inheritance = (true, componentOccurrence);
                    break;

                case ReferenceTypes.Member:
                    this.Member = (true, componentOccurrence);
                    break;

                case ReferenceTypes.Parameter:
                    this.Parameter = (true, componentOccurrence);
                    break;

                default:
                    break;
            }
        }
    }
}
