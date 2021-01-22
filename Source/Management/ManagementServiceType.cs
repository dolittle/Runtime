// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Management
{
    /// <summary>
    /// Represents a <see cref="IRepresentServiceType">service type</see> that is for management communication.
    /// </summary>
    /// <remarks>
    /// Management is considered the channel where tooling is connecting for management.
    /// </remarks>
    public class ManagementServiceType : IRepresentServiceType
    {
        /// <summary>
        /// Gets the identifying name for the <see cref="ManagementServiceType"/>.
        /// </summary>
        public const string Name = "Management";

        /// <inheritdoc/>
        public ServiceType Identifier => Name;

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindManagementServices);

        /// <inheritdoc/>
        public EndpointVisibility Visibility => EndpointVisibility.Public;
    }
}