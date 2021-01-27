// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the configuration typically used by a <see cref="IEndpoint"/>.
    /// </summary>
    public record EndpointConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction server is enabled.
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Gets or sets the port to use for exposing the <see cref="IEndpoint"/> on.
        /// </summary>
        public int Port { get; init; } = 50051;
    }
}