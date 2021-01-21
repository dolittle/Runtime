// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents a <see cref="ICanProvideDefaultConfigurationFor{T}">default provider</see> for <see cref="EndpointsConfiguration"/>.
    /// </summary>
    public class EndpointsConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<EndpointsConfiguration>
    {
        /// <summary>
        /// Accesses the static configurations for providing default <see cref="EndpointConfiguration"/> for different <see cref="ServiceType">service types</see>.
        /// </summary>
        public static readonly Dictionary<EndpointVisibility, EndpointConfiguration> Configurations = new Dictionary<EndpointVisibility, EndpointConfiguration>();

        /// <summary>
        /// The default public port.
        /// </summary>
        public static int DefaultPublicPort = 50052;

        /// <summary>
        /// The default private port.
        /// </summary>
        public static int DefaultPrivatePort = 50053;

        /// <inheritdoc/>
        public EndpointsConfiguration Provide()
        {
            Configurations[EndpointVisibility.Public] = new EndpointConfiguration(DefaultPublicPort);
            Configurations[EndpointVisibility.Private] = new EndpointConfiguration(DefaultPrivatePort);
            return new EndpointsConfiguration(Configurations);
        }
    }
}