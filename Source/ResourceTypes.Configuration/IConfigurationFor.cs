// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Represents a configuration for a <see cref="IRepresentAResourceType"> resource representation</see>.
    /// </summary>
    /// <typeparam name="T">The type of the Configuration.</typeparam>
    public interface IConfigurationFor<T>
        where T : class
    {
        /// <summary>
        /// Gets the configuration instance.
        /// </summary>
        T Instance { get; }
    }
}