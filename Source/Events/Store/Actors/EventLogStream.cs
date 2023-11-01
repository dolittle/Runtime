// Copyright (c) Dolittle. All rights reserved.
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
    const int ChannelSize = 2;

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
        string subscriptionName,
        CancellationToken cancellationToken)
    {
        if (!eventTypes.Any())
        {
            throw new ArgumentException("No event types passed");
        }

        return StartSubscription(scope, from, eventTypes, subscriptionName, cancellationToken);
    }

    public ChannelReader<EventLogBatch> SubscribeAll(
        ScopeId scope,
        EventLogSequenceNumber from,
        string subscriptionName,
        CancellationToken cancellationToken) =>
        StartSubscription(scope, from, ImmutableList<ArtifactId>.Empty, subscriptionName, cancellationToken);

    public ChannelReader<EventLogBatch> SubscribePublic(EventLogSequenceNumber from, CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<EventLogBatch>(ChannelSize);
        var filterConfig = new EventLogStreamActor.FilterConfig(ImmutableArray<ArtifactId>.Empty, true);
        _actorSystem.Root.Spawn(_createProps.PropsFor<EventLogStreamActor>(channel.Writer, ScopeId.Default, from, filterConfig, cancellationToken));
        return channel.Reader;
    }

    ChannelReader<EventLogBatch> StartSubscription(ScopeId scope,
        EventLogSequenceNumber from,
        IReadOnlyCollection<ArtifactId> eventTypes,
        string subscriptionName,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<EventLogBatch>(ChannelSize);
        var filterConfig = new EventLogStreamActor.FilterConfig(eventTypes.ToImmutableHashSet(), subscriptionName: subscriptionName);
        _actorSystem.Root.Spawn(_createProps.PropsFor<EventLogStreamActor>(channel.Writer, scope, from, filterConfig, cancellationToken));
        return channel.Reader;
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class EventLogStreamActor : IActor
{
    public record FilterConfig(IReadOnlyCollection<ArtifactId> _eventTypes, bool publicOnly = false, string subscriptionName = "unnamed");

    readonly ChannelWriter<EventLogBatch> _channelWriter;
    readonly CancellationToken _cancellationToken;

    EventLogSequenceNumber _nextOffset;
    readonly IReadOnlyCollection<ArtifactId> _eventTypes;
    readonly EventStoreClient _eventStoreClient;
    readonly ILogger<EventLogStreamActor> _logger;
    readonly Uuid _subscriptionId;
    readonly Uuid _scope;
    readonly bool _publicOnly;
    readonly string _subscriptionName;

    public EventLogStreamActor(
        ChannelWriter<EventLogBatch> channelWriter,
        ScopeId scope,
        EventLogSequenceNumber nextOffset,
        FilterConfig filterConfig,
        EventStoreClient eventStoreClient,
        ILogger<EventLogStreamActor> logger,
        CancellationToken cancellationToken)
    {
        _channelWriter = channelWriter;
        _nextOffset = nextOffset;
        _eventTypes = filterConfig._eventTypes;
        _publicOnly = filterConfig.publicOnly;
        _subscriptionName = filterConfig.subscriptionName;
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
            SubscriptionWasReset request => OnReset(request, context),
            _ => Task.CompletedTask
        };
    }

    async Task OnReset(SubscriptionWasReset request, IContext context)
    {
        if (!context.CancellationToken.IsCancellationRequested)
        {
            _logger.LogError("Subscription {SubscriptionId} was reset because {Reason}", request.SubscriptionId, request.Reason);
            _channelWriter.Complete(new SubscriptionError(request.Reason));
            // ReSharper disable once MethodHasAsyncOverload
            context.Stop(context.Self);
        }
    }

    async Task OnSubscriptionEvents(SubscriptionEvents request, IContext context)
    {
        try
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
        catch (OperationCanceledException)
        {
            // ReSharper disable once MethodHasAsyncOverload
            // Subscription cancelled
            context.Stop(context.Self);
        }
    }

    static void Ack(EventLogSequenceNumber nextOffset, IContext context) =>
        context.Respond(new SubscriptionEventsAck
        {
            ContinueFromOffset = nextOffset,
        });

    async Task OnStopping()
    {
        _channelWriter.TryComplete();
        var cancelEventStoreSubscription = new CancelEventStoreSubscription
        {
            ScopeId = _scope,
            SubscriptionId = _subscriptionId
        };
        await _eventStoreClient.CancelSubscription(cancelEventStoreSubscription, CancellationTokens.FromSeconds(5)).ConfigureAwait(false);
    }

    async Task OnStarted(IContext context)
    {
        await RegisterSubscription(context);
        context.ReenterAfterCancellation(_cancellationToken, () =>
        {
            if (!context.CancellationToken.IsCancellationRequested)
            {
                context.Stop(context.Self);
            }
        });
    }

    async Task RegisterSubscription(IContext context)
    {
        var eventStoreSubscriptionRequest = new EventStoreSubscriptionRequest
        {
            FromOffset = _nextOffset,
            EventTypeIds = { _eventTypes.Select(it => it.ToProtobuf()) },
            PidAddress = context.Self.Address,
            PidId = context.Self.Id,
            ScopeId = _scope,
            SubscriptionId = _subscriptionId,
            IncludePublicOnly = _publicOnly,
            SubscriptionName = _subscriptionName
        };
        await _eventStoreClient.RegisterSubscription(eventStoreSubscriptionRequest, _cancellationToken).ConfigureAwait(false);
    }
}
