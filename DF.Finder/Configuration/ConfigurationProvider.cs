using DF.Core.Extensions;
using DF.Core.IO;
using DF.Models;
using DF.Models.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DF.Finder.Configuration
{
    /// <summary>
    /// Provides a configuration model based on the design pattern files that are provided
    /// </summary>
    public class ConfigurationProvider
    {
        /// <summary>
        /// Returns a configuration model based on the given fileinfo list, pointing to the file location
        /// </summary>
        /// <param name="configurationFileInfos">The file information list</param>
        /// <returns>The configuration model</returns>
        public ConfigurationModel GetConfigurationModel(List<string> configurationFiles, bool filePaths)
        {
            var patterns = new List<DesignPattern>();

            foreach (var configurationFile in configurationFiles)
            {
                DesignPattern pattern;

                if (filePaths)
                {
                    pattern = XmlFile.DeserializeXmlFromFile<DesignPattern>(configurationFile);
                }
                else
                {
                    pattern = configurationFile.Deserialize<DesignPattern>();
                }

                if (pattern != null)
                {
                    patterns.Add(pattern);
                }
            }

            return this.GetConfigurationModel(patterns);
        }

        /// <summary>
        /// Returns a configuration model based on a list of design patterns
        /// </summary>
        /// <param name="designPatterns">The design patterns</param>
        /// <returns>The configuration model</returns>
        public ConfigurationModel GetConfigurationModel(List<DesignPattern> designPatterns)
        {
            var model = new ConfigurationModel();

            // validate patterns
            var validationMessages = this.ValidatePatterns(designPatterns);

            if (validationMessages.Count() > 0)
            {
                throw new InvalidDataException($"Invalid configuration found in one or more designpatterns: {string.Join(", ", validationMessages)}");
            }

            // Update the properties on the models
            this.UpdateProperties(designPatterns);

            model.DesignPatterns = designPatterns;
            model.Components = designPatterns.SelectMany(d => d.Components).ToList();

            return model;
        }

        /// <summary>
        /// Add the design pattern name to all components/members, as well as the parent of the element
        /// </summary>
        /// <param name="designPatterns">The design patterns to update</param>
        public void UpdateProperties(List<DesignPattern> designPatterns)
        {
            foreach (var pattern in designPatterns)
            {
                foreach (var component in pattern.Components)
                {
                    component.DesignPatternName = pattern.Name;
                    component.DesignPattern = pattern;

                    foreach (var member in component.Members)
                    {
                        member.DesignPatternName = pattern.Name;
                        member.Parent = component;
                    }
                }
            }
        }

        /// <summary>
        /// Validates the given design patterns
        /// </summary>
        /// <param name="designPatterns">The design patterns</param>
        /// <returns>A list of validation errors, if they are present</returns>
        public List<string> ValidatePatterns(List<DesignPattern> designPatterns)
        {
            var errorMessages = new List<string>();
            var validator = new DesignPatternValidator();

            foreach (var designPattern in designPatterns)
            {
                var validationErrors = validator.Validate(designPattern);

                if (validationErrors.Count > 0)
                {
                    errorMessages.AddRange(validationErrors);
                }
            }

            return errorMessages;
        }
    }
}
