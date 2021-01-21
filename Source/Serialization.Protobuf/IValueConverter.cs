// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Defines a converter that is capable of converting to and from.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Determines whether this <see cref="IValueConverter"/> can convert the specified object type.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/> of object.</param>
        /// <returns>True if it can convert, false if not.</returns>
        bool CanConvert(Type objectType);

        /// <summary>
        /// Gets the type the object type should be serialized as.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/> of object.</param>
        /// <returns><see cref="Type"/> it should serialize as.</returns>
        Type SerializedAs(Type objectType);

        /// <summary>
        /// Convert to the value that will be used for serializing.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        object ConvertTo(object value);

        /// <summary>
        /// Convert to the target value on an object from a serialized version of it.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/> of object.</param>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        object ConvertFrom(Type objectType, object value);
    }
}
