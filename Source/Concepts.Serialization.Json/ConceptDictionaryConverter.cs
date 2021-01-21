// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Reflection;
using Dolittle.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Concepts.Serialization.Json
{
    /// <summary>
    /// Represents a <see cref="JsonConverter"/> that can serialize and deserialize a <see cref="IDictionary{TKey, TValue}">dictionary</see> of <see cref="ConceptAs{T}"/>.
    /// </summary>
    public class ConceptDictionaryConverter : JsonConverter, IRequireSerializer
    {
        readonly ILogger _logger;
        ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptDictionaryConverter"/> class.
        /// </summary>
        /// <param name="logger">For logging.</param>
        public ConceptDictionaryConverter(ILogger logger = null)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsDictionary())
            {
                var keyType = objectType.GetKeyTypeFromDictionary();
                return keyType.IsConcept();
            }

            return false;
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var keyType = objectType.GetKeyTypeFromDictionary();
            var keyValueType = keyType.GetConceptValueType();
            var valueType = objectType.GetValueTypeFromDictionary();
            var dictionary = new Dictionary<object, object>();
            JObject jsonDictionary = JObject.Load(reader);

            foreach (var entry in jsonDictionary.Properties())
            {
                try
                {
                    var kvp = BuildKeyValuePair(entry, keyType, valueType);
                    dictionary.Add(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Error reading json: {ex.Message}");
                    throw;
                }
            }

            try
            {
                var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                var finalDictionary = Activator.CreateInstance(dictionaryType) as IDictionary;
                foreach (var key in dictionary.Keys)
                {
                    finalDictionary[key] = dictionary[key];
                }

                if (objectType.IsReadOnlyDictionary())
                {
                    var constructor = objectType.GetConstructors().First();
                    if (constructor != null)
                    {
                        var parameters = constructor.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType.IsDictionary())
                        {
                            finalDictionary = constructor.Invoke(new[] { finalDictionary }) as IDictionary;
                        }
                        else
                        {
                            var readOnlyDictionaryType = typeof(ReadOnlyDictionary<,>).MakeGenericType(keyType, valueType);
                            finalDictionary = Activator.CreateInstance(readOnlyDictionaryType, finalDictionary) as IDictionary;
                        }
                    }
                }

                return finalDictionary;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error reading json: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = value.GetType();
            IEnumerable keys = (IEnumerable)type.GetProperty("Keys").GetValue(value, null);
            IEnumerable values = (IEnumerable)type.GetProperty("Values").GetValue(value, null);
            IEnumerator valueEnumerator = values.GetEnumerator();

            writer.WriteStartObject();
            foreach (object key in keys)
            {
                valueEnumerator.MoveNext();

                writer.WritePropertyName(key.ToString());
                serializer.Serialize(writer, valueEnumerator.Current);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Adds a <see cref="ISerializer"/> for use when reading json.
        /// </summary>
        /// <param name="serializer">Serializer to use to deserialize complex types.</param>
        public void Add(ISerializer serializer)
        {
            _serializer = serializer;
        }

        KeyValuePair<object, object> BuildKeyValuePair(JProperty prop, Type keyType, Type valueType)
        {
            var key = ConceptFactory.CreateConceptInstance(keyType, prop.Name);
            var valueProp = prop.Value;
            object value;
            if (valueType.IsAPrimitiveType())
                value = valueProp.ToObject(valueType);
            else if (valueType.IsConcept())
                value = ConceptFactory.CreateConceptInstance(valueType, valueProp.ToObject(valueType.GetConceptValueType()));
            else
                value = valueType == typeof(object) ? prop.First() : _serializer.FromJson(valueType, prop.Value.ToString());

            return new KeyValuePair<object, object>(key, value);
        }
    }
}
