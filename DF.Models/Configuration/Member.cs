using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DF.Models.Configuration
{
    /// <summary>
    /// A member of a component
    /// </summary>
    public class Member : IBaseConfigModel
    {
        /// <summary>
        /// Gets or sets the access modifier
        /// </summary>
        public Accessibility AccessModifier { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the design pattern.
        /// </summary>
        public DesignPattern DesignPattern { get; set; }

        /// <summary>
        /// Gets or sets the name of the design pattern.
        /// </summary>
        public string DesignPatternName { get; set; }

        /// <summary>
        /// Gets or sets the type of the member, as defined in the MemberType enumerator
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// Gets or sets the modifier
        /// </summary>
        public Modifier Modifier { get; set; }

        /// <summary>
        /// Gets or sets the occurrence of the class, and because of that: whether it is mandatory or not
        /// </summary>
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the parameters, if applicable for the member
        /// </summary>
        public List<Parameter> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the parent. Used within the visitor, not for the generation of the model.
        /// </summary>
        [XmlIgnore]
        public IBaseConfigModel Parent { get; set; }
    }
}
