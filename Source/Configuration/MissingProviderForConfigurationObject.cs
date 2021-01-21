// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="IConfigurationObject"/> is unresolved and can't be provided.
    /// </summary>
    public class MissingProviderForConfigurationObject : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingProviderForConfigurationObject"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/> missing provider for.</param>
        public MissingProviderForConfigurationObject(Type type)
            : base($"There are no providers for '{type.GetFriendlyConfigurationName()}' - '{type.Name}'")
        {
        }
    }
}
