// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Serialization.Json;

/// <summary>
/// Represents the options for serialization.
/// </summary>
public interface ISerializationOptions
{
    /// <summary>
    /// Gets the flag used for serialization.
    /// </summary>
    SerializationOptionsFlags Flags { get; }

    /// <summary>
    /// Gets additional <see cref="JsonConverter">converters</see>.
    /// </summary>
    IEnumerable<JsonConverter> Converters { get; }

    /// <summary>
    /// Gets a value indicating whether or not to ignore discovered converters.
    /// </summary>
    bool IgnoreDiscoveredConverters { get; }

    /// <summary>
    /// Gets the callback that can be used to work directly with the <see cref="JsonSerializer">Newtonsoft serializer</see>.
    /// </summary>
    Action<JsonSerializer> Callback { get; }

    /// <summary>
    /// Gets whether a property on the given type should be serialized.
    /// </summary>
    /// <param name="type"><see cref="Type"/> of the prroperty.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>true if it should be serialized, false if not.</returns>
    bool ShouldSerializeProperty(Type type, string propertyName);
}