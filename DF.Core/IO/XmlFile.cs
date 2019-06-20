using DF.Core.Extensions;
using System.IO;

namespace DF.Core.IO
{
    /// <summary>
    /// Xml I/O
    /// </summary>
    public static class XmlFile
    {
        /// <summary>
        /// Deserializes the XML from file.
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <returns>The generated model, or null if no file was found.</returns>
        public static T DeserializeXmlFromFile<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return default;
            }

            var fileData = File.ReadAllText(filePath);

            // validate with xsd

            return fileData.Deserialize<T>();
        }
    }
}
