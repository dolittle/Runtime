// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Tenancy;
using Proto;
using Failure = Dolittle.Runtime.Protobuf.Failure;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents the event store grain. 
/// </summary>
[TenantGrain(typeof(EventStoreActor), typeof(EventStoreClient))]
// ReSharper disable once UnusedType.Global
public class EventStore : EventStoreBase
{
    public static readonly Failure EventStoreShuttingDown = new EventStoreUnavailable("Runtime shutting down").ToFailure();
    
    readonly TenantId _tenant;
    readonly ICreateProps _propsFactory;
    readonly ITenants _tenants;
    readonly Dictionary<ScopeId, PID> _streamSubscriptionManagerPIDs = new();
    
    PID? _committer;
    bool FailedToStart => _startupFailure is not null;
    Dolittle.Protobuf.Contracts.Failure? _startupFailure;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="context">The actor context.</param>
    /// <param name="tenantId"></param>
    /// <param name="propsFactory"></param>
    /// <param name="tenants"></param>
    public EventStore(IContext context, TenantId tenantId, ICreateProps propsFactory, ITenants tenants)
        : base(context)
    {
        _tenant = tenantId;
        _propsFactory = propsFactory;
        _tenants = tenants;
    }

    /// <inheritdoc />
    public override Task OnStarted()
    {
        if (!_tenants.All.Contains(_tenant))
        {
            _startupFailure = new TenantNotConfigured(_tenant, _tenants.All).ToFailure();
            return Task.CompletedTask;
        }
        if (!TrySpawnSubscriptionManager(ScopeId.Default, out var eventLogSubscriptionPid, out var error))
        {
            _startupFailure = new EventStoreCouldNotBeStarted(_tenant, error, _tenants.All).ToFailure();
            return Task.CompletedTask;
        }
        if (!Context.TrySpawnNamed(_propsFactory.PropsFor<Committer>(), "committer", out _committer, out error))
        {
            _startupFailure = new EventStoreCouldNotBeStarted(_tenant, error, _tenants.All).ToFailure();
            return Task.CompletedTask;
        }
        Context.Send(_committer, new Committer.EventLogSubscriptionManagerSpawned(eventLogSubscriptionPid));
        return Task.CompletedTask;
    }

    bool TrySpawnSubscriptionManager(ScopeId scope, out PID pid, out Exception error)
    {
        if (!Context.TrySpawnNamed(_propsFactory.PropsFor<StreamSubscriptionManagerActor>(scope), $"{scope}-stream-subscription-manager", out pid, out error))
        {
            return false;
        }
        _streamSubscriptionManagerPIDs[scope] = pid;
        return true;
    }

    /// <inheritdoc />
    public override Task Commit(CommitEventsRequest request, Action<CommitEventsResponse> respond, Action<string> onError)
    {
        return ForwardToCommitter<CommitEventsRequest, CommitEventsResponse>(request, RespondWithFailure);
        Task RespondWithFailure(Failure failure)
        {
            respond(new CommitEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task CommitForAggregate(CommitAggregateEventsRequest request, Action<CommitAggregateEventsResponse> respond, Action<string> onError)
    {
        return ForwardToCommitter<CommitAggregateEventsRequest, CommitAggregateEventsResponse>(request, RespondWithFailure);
        Task RespondWithFailure(Failure failure)
        {
            respond(new CommitAggregateEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    Task ForwardToCommitter<TRequest, TResponse>(TRequest request, Func<Failure, Task> respondFailure)
    {
        if (FailedToStart)
        {
            return respondFailure(_startupFailure!);
        }
        Context.Request(_committer!, request!, Context.Sender);
        return Task.CompletedTask;
    }

    public override Task<CommitExternalEventsResponse> CommitExternal(CommitExternalEventsRequest request)
    {
        var scope = request.ScopeId.ToGuid();
        if (FailedToStart)
        {
            return Task.FromResult(new CommitExternalEventsResponse{Failure = _startupFailure});
        }

        if (!_streamSubscriptionManagerPIDs.TryGetValue(scope, out var pid))
        {
            return Task.FromResult(new CommitExternalEventsResponse{Failure = new Failure($"No active subscription for scope {scope}")});
        }

        IReadOnlyCollection<CommittedEvent> committedEvents = new[]
        {
            request.Event.ToCommittedEvent()
        };
        Context.Send(pid, new Commit(new []
            {
                new CommittedEvents(committedEvents.ToList())
            },
            Array.Empty<CommittedAggregateEvents>(),
            committedEvents,
            request.Event.EventLogSequenceNumber,
            request.Event.EventLogSequenceNumber));

        return Task.FromResult(new CommitExternalEventsResponse());
    }

    public override Task RegisterSubscription(EventStoreSubscriptionRequest request, Action<EventStoreSubscriptionAck> respond, Action<string> onError)
    {
        if (FailedToStart)
        {
            onError(_startupFailure!.Reason);
            return Task.CompletedTask;
        }
        var scope = request.ScopeId.ToGuid();
        if (!_streamSubscriptionManagerPIDs.TryGetValue(scope, out var pid))
        {
            if (!TrySpawnSubscriptionManager(scope, out pid, out var error))
            {
                onError(error.Message);
                return Task.CompletedTask;
            }
        }
        
        Context.RequestReenter(pid, request, (Task<EventStoreSubscriptionAck> task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    respond(task.Result);
                }
                else
                {
                    onError(task.Exception?.Message ?? "Failed while registering subscription");
                }
                return Task.CompletedTask;
            },
            Context.CancellationToken);
        return Task.CompletedTask;
    }

    public override Task CancelSubscription(CancelEventStoreSubscription request, Action<CancelEventStoreSubscriptionAck> respond, Action<string> onError)
    {
        if (FailedToStart)
        {
            onError(_startupFailure!.Reason);
            return Task.CompletedTask;
        }
        var scope = request.ScopeId.ToGuid();
        if (!_streamSubscriptionManagerPIDs.TryGetValue(scope, out var pid))
        {
            onError("Subscription does not exist");
            return Task.CompletedTask;
        }

        Context.RequestReenter(pid, request, (Task<CancelEventStoreSubscriptionAck> task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    respond(task.Result);
                }
                else
                {
                    onError(task.Exception?.Message ?? "Failed while cancelling subscription");
                }

                return Task.CompletedTask;
            },
            Context.CancellationToken);
        return Task.CompletedTask;
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request) => throw new NotImplementedException("Unused");
    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request) => throw new NotImplementedException("Unused");
    public override Task<EventStoreSubscriptionAck> RegisterSubscription(EventStoreSubscriptionRequest request) => throw new NotImplementedException("unused");
    public override Task<CancelEventStoreSubscriptionAck> CancelSubscription(CancelEventStoreSubscription request) =>
        throw new NotImplementedException("unused");
}
