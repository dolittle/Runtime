// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration
{
    /// <summary>
    /// Defines a system that might be capable of providing <see cref="IConfigurationObject"/> instances.
    /// </summary>
    public interface ICanProvideConfigurationObjects
    {
        /// <summary>
        /// Method that gets called to tell if the provider can provide the <see cref="IConfigurationObject">configuration object</see> type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject">configuration object</see>.</param>
        /// <returns>true if it can provide, false if not.</returns>
        bool CanProvide(Type type);

        /// <summary>
        /// Provide an instance of the <see cref="IConfigurationObject">configuration object</see>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject">configuration object</see>.</param>
        /// <returns>Instance of the <see cref="IConfigurationObject">configuration object</see>.</returns>
        object Provide(Type type);
    }
}
