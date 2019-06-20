using System;
using System.IO;
using System.Xml.Serialization;

namespace DF.Core.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The deserialized model, as type T</returns>
        /// <exception cref="InvalidDataException">The value contained an invalid data structure to parse it to a model of type {nameof(T)}.</exception>
        public static T Deserialize<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            try
            {
                using (var reader = new StringReader(value))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"The value contained an invalid data structure to parse it to a model of type {nameof(T)}.", ex);
            }
        }
    }
}
