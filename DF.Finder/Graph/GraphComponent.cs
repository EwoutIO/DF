using DF.Models;
using DF.Models.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DF.Finder.Graph
{
    /// <summary>
    /// The Vertex representation of a Component
    /// </summary>
    public class GraphComponent
    {
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        public Component Component { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is referenced.
        /// </summary>
        public bool IsReferenced { get; set; }

        /// <summary>
        /// Gets or sets the references.
        /// </summary>
        public List<Reference> References { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GraphComponent"/> is singular.
        /// </summary>
        public bool Singular { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphComponent"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        public GraphComponent(Component component)
        {
            this.ComponentName = component.Name;
            this.Component = component;
            this.References = new List<Reference>();
            this.Singular = component.Occurrence.MinimumOccurrence == Constants.One && component.Occurrence.MaximumOccurrence == Constants.One;
        }

        /// <summary>
        /// Adds a reference.
        /// </summary>
        /// <param name="referencedComponent">The referenced component.</param>
        /// <param name="referenceType">Type of the reference.</param>
        /// <param name="componentOccurrence">Type of the occurrence.</param>
        public void AddReference(GraphComponent referencedComponent, ReferenceTypes referenceType, ComponentOccurrence componentOccurrence)
        {
            var reference = this.References.SingleOrDefault(r => r.ReferencedComponent == referencedComponent);

            if (reference != null)
            {
                reference.SetReferenceType(referenceType, componentOccurrence);
            }
            else
            {
                this.References.Add(new Reference(referencedComponent, referenceType, componentOccurrence));
            }
        }
    }
}
