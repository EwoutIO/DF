using DF.Models;
using DF.Models.Configuration;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DF.Finder.Configuration
{
    /// <summary>
    /// Validate the design pattern.
    /// </summary>
    public class DesignPatternValidator
    {
        /// <summary>
        /// The current design pattern.
        /// </summary>
        private DesignPattern designPattern;

        /// <summary>
        /// The errormessages. If the validation finds errors or mistakes, these will be returned.
        /// </summary>
        private List<string> errorMessages;

        /// <summary>
        /// True if at least one required component has been found.
        /// </summary>
        private bool requiredComponent;

        public List<string> Validate(DesignPattern pattern)
        {
            this.errorMessages = new List<string>();
            this.designPattern = pattern;

            this.ValidateDesignPattern(pattern);

            return this.errorMessages;
        }

        /// <summary>
        /// Validate the component, and the underlying elements
        /// </summary>
        /// <param name="component">The component</param>
        private void ValidateComponent(Component component)
        {
            if (string.IsNullOrEmpty(component.Name))
            {
                this.errorMessages.Add($"The component does not have a defined name.");
            }

            // validate inheritances (only one base class allowed, multiple interfaces)
            var baseClassCount = component.Inheritances.Count(i => i.ComponentType == TypeKind.Class);

            if (baseClassCount > 1)
            {
                this.errorMessages.Add($"Component {component.Name} has more than one baseclass defined.");
            }

            foreach (var inheritance in component.Inheritances)
            {
                this.ValidateInheritance(inheritance);
            }

            // validate members
            foreach (var member in component.Members)
            {
                this.ValidateMember(member);
            }

            // validate occurrence
            this.ValidateOccurrence(component.Occurrence);

            // At least one component of the design pattern should be required.
            // To be discussed: how are we going to match patterns otherwise?
            if (component.Occurrence.MinimumOccurrence.Equals(Constants.Zero) == false)
            {
                this.requiredComponent = true;
            }
        }

        /// <summary>
        /// Validate the datatype, and update the type-field, if a type has been found.
        /// Discuss: single responsibility violation? Should we set this after the validation?
        /// </summary>
        /// <param name="dataType">The datatype</param>
        private void ValidateDataType(DataType dataType)
        {
            var type = this.ValidateType(dataType.TypeName, dataType.Custom);

            dataType.Type = type;
        }

        /// <summary>
        /// Validate the design pattern and the underlying elements
        /// </summary>
        /// <param name="designPattern">The design pattern</param>
        private void ValidateDesignPattern(DesignPattern designPattern)
        {
            if (string.IsNullOrEmpty(designPattern.Name))
            {
                this.errorMessages.Add($"The design pattern does not have a defined name.");
                return;
            }

            if (designPattern.Components == null || designPattern.Components.Count < 1)
            {
                this.errorMessages.Add($"The design pattern ({designPattern.Name}) should contain at least one component.");
                return;
            }

            foreach (var component in designPattern.Components)
            {
                this.ValidateComponent(component);
            }

            if (this.requiredComponent == false)
            {
                this.errorMessages.Add($"Designpattern {designPattern.Name} does not have any required components. No definitive matches possible.");
            }
        }

        /// <summary>
        /// Validates the inheritance by checking the type.
        /// </summary>
        /// <param name="inheritance"></param>
        private void ValidateInheritance(Inheritance inheritance)
        {
            // validate type
            var type = this.ValidateType(inheritance.Name, inheritance.Custom);

            inheritance.Type = type;
        }

        /// <summary>
        /// Validates the member.
        /// </summary>
        /// <param name="member">The member</param>
        private void ValidateMember(Member member)
        {
            // validate datatype
            if (member.DataType != null)
            {
                this.ValidateDataType(member.DataType);
            }

            // validate parameters
            foreach (var parameter in member.Parameters)
            {
                this.ValidateParameter(parameter);
            }

            // validate occurrence
            this.ValidateOccurrence(member.Occurrence);
        }

        /// <summary>
        /// Validate the occurrencevalues. They should be one of three values, and the minimum should never be higher than the maximum.
        /// </summary>
        /// <param name="occurrence">The occurrence</param>
        private void ValidateOccurrence(Occurrence occurrence)
        {
            var validValues = new string[] { Constants.Zero, Constants.One, Constants.N };
            var min = occurrence.MinimumOccurrence;
            var max = occurrence.MaximumOccurrence;

            // If the minimum value is zero, the maximum can have any value.
            if (validValues.Contains(occurrence.MinimumOccurrence) == false)
            {
                this.errorMessages.Add($"MinimumOccurrence can not have a value of {min}");
            }

            if (validValues.Contains(occurrence.MaximumOccurrence) == false)
            {
                this.errorMessages.Add($"MaximumOccurrence can not have a value of {max}");
            }

            // if min is N, max must be N
            if (min.Equals(Constants.N) && max.Equals(Constants.N) == false)
            {
                this.errorMessages.Add($"MaximumOccurrence ({max}) must be equal or higher than the MinimumOccurrence ({min})");
            }

            // if min is 1, max must be 1 or N
            if (min.Equals(Constants.One) && max.Equals(Constants.Zero))
            {
                this.errorMessages.Add($"MaximumOccurrence ({max}) must be equal or higher than the MinimumOccurrence ({min})");
            }

            // if min is 0, max can be any
            return;
        }

        /// <summary>
        /// Validate the parameter, by checking its datatype and occurrence.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        private void ValidateParameter(Parameter parameter)
        {
            // validate datatype
            this.ValidateDataType(parameter.DataType);

            // validate the occurrence
            this.ValidateOccurrence(parameter.Occurrence);
        }

        /// <summary>
        /// Validate the type. Either match the name to a component (if custom), or get the type from the system library if not.
        /// </summary>
        /// <param name="typeName">The name of the type</param>
        /// <param name="custom">Whether the type is custom/user-created for this pattern.</param>
        /// <returns>Returns the type if it has been found</returns>
        private Type ValidateType(string typeName, bool custom)
        {
            if (custom)
            {
                // check if the type is available as component
                // return null;
                if (this.designPattern.Components.Any(c => c.Name == typeName) == false)
                {
                    this.errorMessages.Add($"No custom typename of name {typeName} found as name of a component.");
                }

                return null;
            }

            var type = Type.GetType(typeName, false, true);

            if (type == null)
            {
                this.errorMessages.Add($"Systemtype with name {typeName} could not be found.");
            }

            return type;
        }
    }
}
