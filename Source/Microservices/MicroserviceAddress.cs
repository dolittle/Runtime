// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents the address of a <see cref="Microservice" />.
    /// </summary>
    public class MicroserviceAddress
    {
        /// <summary>
        /// Gets or sets the target host for the subscription.
        /// </summary>
        public MicroserviceHost Host { get; set; }

        /// <summary>
        /// Gets or sets the target port for the subscription.
        /// </summary>
        public MicroservicePort Port { get; set; }
    }
}