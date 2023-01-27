// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamSubscriptionActor), typeof(StreamSubscriptionStateClient))]
public class StreamSubscriptionStateManager : StreamSubscriptionStateBase
{
    readonly ISubscriptionStateRepository _repository;
    readonly ILogger<StreamSubscriptionStateManager> _logger;
    readonly IApplicationLifecycleHooks _lifecycleHooks;


    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _subscriptionStates = new();
    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _changedSubscriptionStates = new();

    readonly HashSet<ScopeId> _activeRequests = new();
    readonly Dictionary<StreamSubscriptionId, Task<Bucket>> _waitingForSubscriptionStates = new();


    IShutdownHook _shutdownHook;
    bool _shuttingDown;


    public StreamSubscriptionStateManager(IContext context, ISubscriptionStateRepository repository, ILogger<StreamSubscriptionStateManager> logger,
        IApplicationLifecycleHooks lifecycleHooks) : base(context)
    {
        _repository = repository;
        _logger = logger;
        _lifecycleHooks = lifecycleHooks;
    }

    public override Task OnStarted()
    {
        _shutdownHook = _lifecycleHooks.RegisterShutdownHook();
        Context.ReenterAfter(_shutdownHook.ShuttingDown, () =>
        {
            _shuttingDown = true;
            if (_activeRequests.Count == 0) // No current changes
            {
                _shutdownHook.MarkCompleted();
            }
        });
        return Task.CompletedTask;
    }


    public override Task GetBySubscriptionId(StreamSubscriptionId subscriptionId, Action<StreamProcessorStateResponse> respond, Action<string> onError)
    {
        if (_shuttingDown)
        {
            onError("Runtime is shutting down");
            return Task.CompletedTask;
        }

        if (!_waitingForSubscriptionStates.TryGetValue(subscriptionId, out var resultTask))
        {
            resultTask = LoadSubscription(subscriptionId);
        }

        Context.ReenterAfter(resultTask, _ =>
        {
            if (_.IsCompletedSuccessfully)
            {
                respond(new StreamProcessorStateResponse
                {
                    StreamKey = new StreamProcessorKey
                    {
                        SubscriptionId = subscriptionId
                    },
                    Bucket = { resultTask!.Result }
                });
            }
            else
            {
                onError("Failed to get subscription state: " + resultTask.Exception?.Message);
            }
        });
        return Task.CompletedTask;
    }


    Task<Bucket> LoadSubscription(StreamSubscriptionId subscriptionId)
    {
        var taskCompletionSource = new TaskCompletionSource<Bucket>();
        var task = _repository.TryGet(SubscriptionId.FromProtobuf(subscriptionId), Context.CancellationToken);
        _waitingForSubscriptionStates.Add(subscriptionId, taskCompletionSource.Task);
        Context.ReenterAfter(task, _ =>
        {
            try
            {
                if (task is { IsCompletedSuccessfully: true, Result.Success: true })
                {
                    taskCompletionSource.SetResult(task.Result.Result.ToProtobuf());
                }
                else
                {
                    if (task.Exception is not null)
                    {
                        taskCompletionSource.SetException(task.Exception);
                    }
                }
            }
            finally
            {
                _waitingForSubscriptionStates.Remove(subscriptionId);
            }
        });
        return taskCompletionSource.Task;
    }

    public override Task SetBySubscriptionId(SetStreamProcessorStateRequest request)
    {
        if (request.StreamKey.IdCase != StreamProcessorKey.IdOneofCase.SubscriptionId)
        {
            throw new ArgumentException("Can only set subscription state by subscription id");
        }

        var subscriptionId = request.StreamKey.SubscriptionId;
        ScopeId scopeId = subscriptionId.ScopeId.ToGuid();

        SetSubscriptionState(scopeId, subscriptionId, request.Bucket);
        AddChange(scopeId, subscriptionId, request.Bucket);

        PersistCurrentSubscriptionState(scopeId);
        return Task.CompletedTask;
    }

    void SetSubscriptionState(ScopeId scope, StreamSubscriptionId subscriptionId, Bucket state)
    {
        if (!_subscriptionStates.TryGetValue(scope, out var scopedSubscriptionStates))
        {
            _subscriptionStates.Add(scope, scopedSubscriptionStates = new());
        }

        scopedSubscriptionStates[subscriptionId] = state;
    }

    void AddChange(ScopeId scope, StreamSubscriptionId subscriptionId, Bucket state)
    {
        if (!_changedSubscriptionStates.TryGetValue(scope, out var changedScopedSubscriptionStates))
        {
            _changedSubscriptionStates.Add(scope, changedScopedSubscriptionStates = new());
        }

        changedScopedSubscriptionStates[subscriptionId] = state;
    }

    void PersistCurrentSubscriptionState(ScopeId scopeId)
    {
        if (_activeRequests.Contains(scopeId))
        {
            return;
        }

        if (_changedSubscriptionStates.Count == 0) return;
        if (!_changedSubscriptionStates.Remove(scopeId, out var currentChanges))
        {
            return; // No changes for this scope
        }

        _activeRequests.Add(scopeId);
        var changes = FromProtobuf(currentChanges);

        Persist(changes, scopeId);
    }

    void Persist(IReadOnlyDictionary<SubscriptionId, StreamProcessorState> changes, ScopeId scopeId)
    {
        var persistTask = _repository.PersistForScope(scopeId, changes, Context.CancellationToken);
        Context.ReenterAfter(persistTask, _ =>
        {
            if (persistTask.IsCompletedSuccessfully)
            {
                _activeRequests.Remove(scopeId);
                PersistCurrentSubscriptionState(scopeId);
                if (_shuttingDown && _activeRequests.Count == 0)
                {
                    _shutdownHook.MarkCompleted();
                }
            }
            else
            {
                _logger.FailedToPersistStreamSubscriptionState(persistTask.Exception!, scopeId);
                // Try again
                Persist(changes, scopeId);
            }
        });
    }

    IReadOnlyDictionary<SubscriptionId, StreamProcessorState> FromProtobuf(IDictionary<StreamSubscriptionId, Bucket> changes)
    {
        var dict = new Dictionary<SubscriptionId, StreamProcessorState>();
        foreach (var change in changes)
        {
            var state = change.Value.FromProtobuf();
            if (state is StreamProcessorState streamProcessorState)
            {
                dict.Add(SubscriptionId.FromProtobuf(change.Key), streamProcessorState);
            }
            else
            {
                _logger.LogWarning("Unecpected state type: {stateType}", state.GetType().FullName);
            }
        }

        return dict;
    }

    #region Unused, handled with reentrant overloads

    public override Task<StreamProcessorStateResponse> GetBySubscriptionId(StreamSubscriptionId request) => throw new NotImplementedException("unused");

    #endregion
}
