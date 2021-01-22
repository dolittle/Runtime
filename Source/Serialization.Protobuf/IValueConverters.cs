// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Defines a system for working with <see cref="IValueConverter">value converters</see>.
    /// </summary>
    public interface IValueConverters
    {
        /// <summary>
        /// Check wether or not a specific type can be converted.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to ask for if can convert.</param>
        /// <returns>true if it can convert, false if not.</returns>
        bool CanConvert(Type type);

        /// <summary>
        /// Get the <see cref="IValueConverter"/> for a given type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get for.</param>
        /// <returns><see cref="IValueConverter"/> for the <see cref="Type"/>.</returns>
        IValueConverter GetConverterFor(Type type);
    }
}