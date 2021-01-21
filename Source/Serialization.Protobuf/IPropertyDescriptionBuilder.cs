// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Defines a builder for building <see cref="PropertyDescription"/>.
    /// </summary>
    public interface IPropertyDescriptionBuilder
    {
        /// <summary>
        /// Builld a <see cref="PropertyDescription"/>.
        /// </summary>
        /// <returns>A new <see cref="PropertyDescription"/> instance.</returns>
        PropertyDescription Build();
    }
}