using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DF.Models.Configuration
{
    /// <summary>
    /// One of the components of a design pattern
    /// </summary>
    public class Component : IBaseConfigModel
    {
        public Accessibility AccessModifier { get; set; }

        /// <summary>
        /// Gets or sets the type of the component
        /// </summary>
        public TypeKind ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the design pattern.
        /// </summary>
        public DesignPattern DesignPattern { get; set; }

        /// <summary>
        /// Gets or sets the name of the design pattern.
        /// </summary>
        public string DesignPatternName { get; set; }

        /// <summary>
        /// Gets or sets a list of interfaces and/or parent classes
        /// </summary>
        public List<Inheritance> Inheritances { get; set; }

        /// <summary>
        /// Gets or sets a list of members: methods, properties, etc
        /// </summary>
        public List<Member> Members { get; set; }

        /// <summary>
        /// Gets or sets the modifier
        /// </summary>
        public Modifier Modifier { get; set; }

        /// <summary>
        /// Gets or sets the name of the component
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the occurrence of the class, and because of that: whether it is mandatory or not
        /// </summary>
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the parent. Used within the visitor, not for the generation of the model.
        /// </summary>
        [XmlIgnore]
        public IBaseConfigModel Parent { get; set; }
    }
}
