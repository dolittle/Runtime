// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using MongoSubscriptionState = Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon.SubscriptionState;
using UnPartitionedStreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="Store.Streams.IStreamProcessorStates" />.
/// </summary>
[PerTenant]
public class SubscriptionStateRepository : StreamProcessorStateRepositoryBase<SubscriptionId, UnPartitionedStreamProcessorState, MongoSubscriptionState>, ISubscriptionStateRepository
{
    readonly TenantId _tenant;
    readonly FilterDefinitionBuilder<MongoSubscriptionState> _subscriptionFilter = Builders<MongoSubscriptionState>.Filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="tenant">The tenant id.</param>
    /// <param name="subscriptionStateCollections">The <see cref="ISubscriptionStateCollections" />.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public SubscriptionStateRepository(
        TenantId tenant,
        ISubscriptionStateCollections subscriptionStateCollections,
        ILogger logger)
        : base(subscriptionStateCollections.Get, logger)
    {
        _tenant = tenant;
    }

    protected override FilterDefinition<MongoSubscriptionState> CreateFilter(SubscriptionId id) =>
        _subscriptionFilter.Eq(_ => _.Microservice, id.ProducerMicroserviceId.Value)
        & _subscriptionFilter.Eq(_ => _.Tenant, id.ProducerTenantId.Value)
        & _subscriptionFilter.Eq(_ => _.Stream, id.StreamId.Value)
        & _subscriptionFilter.EqStringOrGuid(_ => _.Partition, id.PartitionId.Value);

    protected override MongoSubscriptionState CreateDocument(SubscriptionId id, UnPartitionedStreamProcessorState state)
        => new(
            id.ProducerMicroserviceId,
            id.ProducerTenantId,
            id.StreamId,
            id.PartitionId,
            state.Position.StreamPosition,
            state.Position.EventLogPosition,
            state.RetryTime.UtcDateTime,
            state.FailureReason,
            state.ProcessingAttempts,
            state.LastSuccessfullyProcessed.UtcDateTime,
            state.IsFailing);

    protected override StreamProcessorStateWithId<SubscriptionId, UnPartitionedStreamProcessorState> ConvertToStateWithId(ScopeId scope, MongoSubscriptionState document)
        => new (new SubscriptionId(_tenant, document.Microservice, document.Tenant, scope, document.Stream, document.Partition), document.ToRuntimeRepresentation());
}
