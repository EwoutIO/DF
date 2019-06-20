namespace DF.Models
{
    /// <summary>
    /// Possible values for inheritance/use of a custom type.
    /// </summary>
    public enum ComponentOccurrence
    {
        Mandatory,
        Possible,
        Prohibited
    }

    /// <summary>
    /// Defines the member type
    /// </summary>
    public enum MemberType
    {
        Any,
        Constructor,
        Constant,
        Field,
        Finalizer,
        Method,
        Property,
        Indexer,
        Operator,
        Event,
        Delegate,
        Class,
        Interface,
        Struct,
        Enum
    }

    /// <summary>
    /// Defines the modifier of the object
    /// </summary>
    /// <remarks>
    /// TODO: Implement modifier as an array in the model, member can have more than one modifier (sealed override for ex.)
    /// Will be done later, in version 1.1/2.0.
    /// </remarks>
    public enum Modifier
    {
        Any,
        Abstract,
        Sealed,
        Virtual,
        Static,
        Override
    }

    /// <summary>
    /// Locations that the parent can be referred to in the child.
    /// </summary>
    public enum ReferenceTypes
    {
        /// <summary>
        /// Indicates that the parent is inherited (as baseclass or interface)
        /// </summary>
        Inheritance,

        /// <summary>
        /// Indicates that the parent is used as a type of a member, or as a returntype of a member.
        /// </summary>
        Member,

        /// <summary>
        /// Indicates that the parent is used as a type of a parameter of a (method)member.
        /// </summary>
        Parameter
    }
}
