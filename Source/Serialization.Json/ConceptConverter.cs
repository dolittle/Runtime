// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Reflection;
using Dolittle.Runtime.Rudimentary;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Serialization.Json;

/// <summary>
/// Represents an implementation of <see cref="JsonConverter"/> for dealing with <see cref="ConceptAs{T}"/>.
/// </summary>
public class ConceptConverter : JsonConverter
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => objectType.IsDerivedFromOpenGeneric(typeof(ConceptAs<>));

    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var valueType = objectType.GetTypeInfo().BaseType?.GetGenericArguments()[0];
        var value = serializer.Deserialize(reader, valueType);
        var constructor = objectType.GetConstructor(new[] { valueType });

        return constructor.Invoke(new object[] { value });
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.GetType().GetProperty(nameof(ConceptAs<object>.Value))?.GetValue(value));
    }
}