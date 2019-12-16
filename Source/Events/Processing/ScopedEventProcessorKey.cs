// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Concepts;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// A key to mark the <see cref="Artifact">Event Artifact</see> and <see cref="TenantId">Tenant</see> scope.
    /// </summary>
    public class ScopedEventProcessorKey : Value<ScopedEventProcessorKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedEventProcessorKey"/> class.
        /// </summary>
        /// <param name="tenant"><see cref="TenantId"/> part of the key.</param>
        /// <param name="event"><see cref="Artifact"/> part of the key.</param>
        public ScopedEventProcessorKey(TenantId tenant, Artifact @event)
        {
            Event = @event;
            Tenant = tenant;
        }

        /// <summary>
        /// Gets the <see cref="Artifact"/> representing the Event.
        /// </summary>
        public Artifact Event { get; }

        /// <summary>
        /// Gets the <see cref="TenantId">id</see> of the Tenant.
        /// </summary>
        public TenantId Tenant { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Event} - {Tenant}";
        }
    }
}