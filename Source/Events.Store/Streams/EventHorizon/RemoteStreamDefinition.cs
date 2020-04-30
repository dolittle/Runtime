// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ApplicationModel;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Store.Streams.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamDefinition" /> that defines a remote stream.
    /// </summary>
    public class RemoteStreamDefinition : IStreamDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteStreamDefinition"/> class.
        /// </summary>
        /// <param name="microserviceId">The <see cref="Microservice" />.</param>
        /// <param name="tenantId">The <see cref="TenantId" />.</param>
        /// <param name="publicStreamId">The public <see cref="StreamId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        public RemoteStreamDefinition(Microservice microserviceId, TenantId tenantId, StreamId publicStreamId, PartitionId partitionId)
        {
            MicroserviceId = microserviceId;
            TenantId = tenantId;
            PublicStreamId = publicStreamId;
            PartitionId = partitionId;
        }

        /// <summary>
        /// Gets the <see cref="Microservice" /> that the stream originates from.
        /// </summary>
        public Microservice MicroserviceId { get; }

        /// <summary>
        /// Gets the <see cref="TenantId" /> that the stream originates from.
        /// </summary>
        public TenantId TenantId { get; }

        /// <summary>
        /// Gets the public <see cref="StreamId">stream</see> that stream originates from.
        /// </summary>
        public StreamId PublicStreamId { get; }

        /// <summary>
        /// Gets the <see cref="Streams.PartitionId" /> in the Public Stream that the stream originates from.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <inheritdoc/>
        public bool Partitioned => true;

        /// <inheritdoc/>
        public bool Public => false;
    }
}
