﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="IEventLogStream"/>.
/// </summary>
[Singleton, PerTenant]
public class EventLogStream : IEventLogStream
{
    const int ChannelSize = 100;

    readonly ActorSystem _actorSystem;
    readonly ICreateProps _createProps;

    public EventLogStream(ActorSystem actorSystem, ICreateProps createProps)
    {
        _actorSystem = actorSystem;
        _createProps = createProps;
    }

    public ChannelReader<EventLogBatch> Subscribe(
        ScopeId scope,
        EventLogSequenceNumber from,
        IReadOnlyCollection<ArtifactId> eventTypes,
        CancellationToken cancellationToken)
    {
        if (!eventTypes.Any())
        {
            throw new ArgumentException("No event types passed");
        }

        return StartSubscription(scope, from, eventTypes, cancellationToken);
    }

    public ChannelReader<EventLogBatch> SubscribeAll(
        ScopeId scope,
        EventLogSequenceNumber from,
        CancellationToken cancellationToken) =>
        StartSubscription(scope, from, ImmutableList<ArtifactId>.Empty, cancellationToken);

    ChannelReader<EventLogBatch> StartSubscription(
        ScopeId scope,
        EventLogSequenceNumber from,
        IReadOnlyCollection<ArtifactId> eventTypes,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<EventLogBatch>(ChannelSize);
        _actorSystem.Root.Spawn(_createProps.PropsFor<EventLogStreamActor>(channel.Writer, scope, from, eventTypes, cancellationToken));
        return channel.Reader;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class EventLogStreamActor : IActor
{
    readonly ChannelWriter<EventLogBatch> _channelWriter;
    readonly CancellationToken _cancellationToken;

    EventLogSequenceNumber _nextOffset;
    readonly IReadOnlyCollection<ArtifactId> _eventTypes;
    readonly EventStoreClient _eventStoreClient;
    readonly ILogger<EventLogStreamActor> _logger;
    readonly Uuid _subscriptionId;
    readonly Uuid _scope;

    public EventLogStreamActor(
        ChannelWriter<EventLogBatch> channelWriter,
        ScopeId scope,
        EventLogSequenceNumber nextOffset,
        IReadOnlyCollection<ArtifactId> eventTypeIds,
        EventStoreClient eventStoreClient,
        ILogger<EventLogStreamActor> logger,
        CancellationToken cancellationToken)
    {
        _channelWriter = channelWriter;
        _nextOffset = nextOffset;
        _eventTypes = eventTypeIds;
        _eventStoreClient = eventStoreClient;
        _logger = logger;
        _cancellationToken = cancellationToken;
        _subscriptionId = Guid.NewGuid().ToProtobuf();
        _scope = scope.ToProtobuf();
    }

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            Started => OnStarted(context),
            Stopping => OnStopping(),
            SubscriptionEvents request => OnSubscriptionEvents(request, context),
            _ => Task.CompletedTask
        };
    }

    async Task OnSubscriptionEvents(SubscriptionEvents request, IContext context)
    {
        if (request.FromOffset == _nextOffset)
        {
            _nextOffset = request.ToOffset + 1;
            Ack(_nextOffset, context);
            await _channelWriter.WriteAsync(new EventLogBatch(
                request.FromOffset,
                request.ToOffset,
                request.Events), _cancellationToken).ConfigureAwait(false);
        }
        else
        {
            Ack(_nextOffset, context);
            _logger.LogUnexpectedOffset(_nextOffset, request.FromOffset);
        }
    }

    static void Ack(EventLogSequenceNumber nextOffset, IContext context) =>
        context.Respond(new SubscriptionEventsAck
        {
            ContinueFromOffset = nextOffset,
        });

    async Task OnStopping()
    {
        _channelWriter.Complete();
        await _eventStoreClient.CancelSubscription(new CancelEventStoreSubscription
        {
            SubscriptionId = _subscriptionId
        }, CancellationTokens.FromSeconds(5)).ConfigureAwait(false);
    }

    async Task OnStarted(IContext context)
    {
        var eventStoreSubscriptionRequest = new EventStoreSubscriptionRequest
        {
            FromOffset = _nextOffset,
            EventTypeIds = { _eventTypes.Select(it => it.ToProtobuf()) },
            PidAddress = context.Self.Address,
            PidId = context.Self.Id,
            ScopeId = _scope,
            SubscriptionId = _subscriptionId
        };
        await _eventStoreClient.RegisterSubscription(eventStoreSubscriptionRequest, _cancellationToken).ConfigureAwait(false);
        context.ReenterAfterCancellation(_cancellationToken, () => context.Stop(context.Self));
    }
}