using DF.Models;
using DF.Models.Configuration;
using DF.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DF.Finder.Graph
{
    /// <summary>
    /// The Graph representation of the Pattern
    /// </summary>
    public class PatternGraph
    {
        /// <summary>
        /// Gets or sets the design pattern.
        /// </summary>
        public DesignPattern DesignPattern { get; set; }

        /// <summary>
        /// The components
        /// </summary>
        private readonly List<GraphComponent> components = new List<GraphComponent>();

        /// <summary>
        /// The inheritance occurrence default.
        /// </summary>
        private readonly Occurrence inheritanceOccurrence = new Occurrence() { MinimumOccurrence = Constants.One, MaximumOccurrence = Constants.One };

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternGraph"/> class.
        /// </summary>
        /// <param name="designPattern">The design pattern.</param>
        public PatternGraph(DesignPattern designPattern)
        {
            this.DesignPattern = designPattern;
            this.Initialize();
        }

        /// <summary>
        /// Adds a component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void AddComponent(Component component)
        {
            this.components.Add(new GraphComponent(component));
        }

        /// <summary>
        /// Adds a reference.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <param name="referencedName">Name of the referenced.</param>
        /// <param name="referenceType">Type of the reference.</param>
        public void AddReference(string sourceName, string referencedName, ReferenceTypes referenceType, ComponentOccurrence componentOccurrence)
        {
            var sourceComponent = this.GetGraphComponent(sourceName);
            var referencedComponent = this.GetGraphComponent(referencedName);
            sourceComponent.AddReference(referencedComponent, referenceType, componentOccurrence);
        }

        /// <summary>
        /// Gets a graph component.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns>The corresponding graph component</returns>
        public GraphComponent GetGraphComponent(string componentName) => this.components.SingleOrDefault(c => c.ComponentName == componentName);

        /// <summary>
        /// Gets graph components.
        /// </summary>
        /// <param name="componentNames">Name of the components.</param>
        /// <returns>The corresponding graph components</returns>
        public IEnumerable<GraphComponent> GetGraphComponents(IEnumerable<string> componentNames) => this.components.Where(c => componentNames.Contains(c.ComponentName));

        /// <summary>
        /// Gets singular component names.
        /// </summary>
        /// <returns>A list of singular components (that occur min/max once)</returns>
        public List<string> GetSingularComponentNames() => this.components.Where(c => c.Singular).Select(c => c.ComponentName).ToList();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            // Add components
            foreach (var component in this.DesignPattern.Components)
            {
                this.AddComponent(component);
            }

            // Set up references/edges
            foreach (var graphComponent in this.components)
            {
                var inheritedComponents = this.GetInheritedComponents(graphComponent.Component);

                foreach (var (componentName, occurrence) in inheritedComponents.inheritances)
                {
                    this.AddReference(graphComponent.ComponentName, componentName, ReferenceTypes.Inheritance, occurrence);
                }

                foreach (var (componentName, occurrence) in inheritedComponents.members)
                {
                    this.AddReference(graphComponent.ComponentName, componentName, ReferenceTypes.Member, occurrence);
                }

                foreach (var (componentName, occurrence) in inheritedComponents.parameters)
                {
                    this.AddReference(graphComponent.ComponentName, componentName, ReferenceTypes.Parameter, occurrence);
                }
            }
        }

        /// <summary>
        /// Gets the component occurrence.
        /// </summary>
        /// <param name="occurrence">The occurrence.</param>
        /// <returns>Whether the occurrence is mandatory, prohibited or possible</returns>
        private ComponentOccurrence GetComponentOccurrence(Occurrence occurrence)
        {
            if (occurrence.MinimumOccurrence == Constants.One) return ComponentOccurrence.Mandatory;
            if (occurrence.MaximumOccurrence == Constants.Zero) return ComponentOccurrence.Prohibited;
            return ComponentOccurrence.Possible;
        }

        /// <summary>
        /// Get lists of inheritance-names of the component.
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>The components that this component inherits, in three lists, depending on where they inherit them</returns>
        private InheritanceNames GetInheritedComponents(Component component)
        {
            return new InheritanceNames
            {
                inheritances = component.Inheritances.Where(i => i.Custom && i.Name != component.Name).Select(i => (i.Name, this.GetComponentOccurrence(this.inheritanceOccurrence))),
                members = component.Members.Where(m => m.DataType?.Custom == true && m.DataType.TypeName != component.Name).Select(m => (m.DataType.TypeName, this.GetComponentOccurrence(m.Occurrence))),
                parameters = component.Members.SelectMany(m => m.Parameters).Where(p => p.DataType?.Custom == true && p.DataType.TypeName != component.Name).Select(p => (p.DataType.TypeName, this.GetComponentOccurrence(p.Occurrence)))
            };
        }
    }
}
