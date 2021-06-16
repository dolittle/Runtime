// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Reflection
{
    /// <summary>
    /// Defines something can deal with creating proxy types.
    /// </summary>
    public interface IProxying
    {
        /// <summary>
        /// Build an interface type that contains the properties from a specific other type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get properties from.</param>
        /// <returns>A new <see cref="Type"/>.</returns>
        Type BuildInterfaceWithPropertiesFrom(Type type);

        /// <summary>
        /// Build a class type that contains the properties from a specific other type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get properties from.</param>
        /// <returns>A new <see cref="Type"/>.</returns>
        Type BuildClassWithPropertiesFrom(Type type);
    }
}
