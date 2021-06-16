// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration
{
    /// <summary>
    /// Exception that gets thrown when there are multiple providers providing a specific <see cref="IConfigurationObject"/>.
    /// </summary>
    public class MultipleProvidersProvidingConfigurationObject : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleProvidersProvidingConfigurationObject"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/>.</param>
        public MultipleProvidersProvidingConfigurationObject(Type type)
            : base($"There are multiple providers that can provide the configuration object '{type.GetFriendlyConfigurationName()}' - '{type.Name}'")
        {
        }
    }
}
