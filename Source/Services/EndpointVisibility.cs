// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the visibility for an <see cref="IEndpoint"/>.
    /// </summary>
    public enum EndpointVisibility
    {
        /// <summary>
        /// Represents public endpoints
        /// </summary>
        Public = 1,

        /// <summary>
        /// Represents private endpoints
        /// </summary>
        Private
    }
}