using Microsoft.CodeAnalysis;

namespace DF.Models.Configuration
{
    /// <summary>
    /// Interface to combine the Component and the Member-classes, for easier parsing.
    /// </summary>
    public interface IBaseConfigModel
    {
        /// <summary>
        /// Gets or sets the access modifier
        /// </summary>
        Accessibility AccessModifier { get; set; }

        /// <summary>
        /// Gets or sets the name of the design pattern.
        /// </summary>
        string DesignPatternName { get; set; }

        /// <summary>
        /// Gets or sets the modifier
        /// </summary>
        Modifier Modifier { get; set; }

        /// <summary>
        /// Gets or sets the occurrence of the class, and because of that: whether it is mandatory or not
        /// </summary>
        Occurrence Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the parent. Used within the visitor, not for the generation of the model.
        /// </summary>
        IBaseConfigModel Parent { get; set; }
    }
}
