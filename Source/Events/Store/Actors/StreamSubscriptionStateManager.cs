// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamSubscriptionStateActor), typeof(StreamSubscriptionStateClient))]
public class StreamSubscriptionStateManager : StreamSubscriptionStateBase
{
    readonly ISubscriptionStateRepository _repository;
    readonly ILogger<StreamSubscriptionStateManager> _logger;
    readonly IApplicationLifecycleHooks _lifecycleHooks;


    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _subscriptionStates = new();
    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _changedSubscriptionStates = new();

    readonly HashSet<ScopeId> _activeRequests = new();
    readonly Dictionary<ScopeId, Task> _loadingScopes = new();


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
        LoadScope(ScopeId.Default);
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

        ScopeId scopeId = subscriptionId.ScopeId.ToGuid();
        if (TrySendResponse(scopeId, subscriptionId, respond))
        {
            return Task.CompletedTask;
        }

        if (!_loadingScopes.TryGetValue(scopeId, out var scopeHasLoadedTask))
        {
            scopeHasLoadedTask = LoadScope(scopeId);
        }


        Context.ReenterAfter(scopeHasLoadedTask, _ =>
        {
            if (_.IsCompletedSuccessfully && TrySendResponse(scopeId, subscriptionId, respond))
            {
                // All OK
            }
            else
            {
                onError("Failed to get subscription state: " + scopeHasLoadedTask.Exception?.Message);
            }
        });
        return Task.CompletedTask;
    }

    bool TrySendResponse(ScopeId scopeId, StreamSubscriptionId subscriptionId, Action<StreamProcessorStateResponse> respond)
    {
        {
            if (!_subscriptionStates.TryGetValue(scopeId, out var scopeStates))
            {
                // Scope has not been loaded
                return false;
            }

            // The scope is already loaded
            SendResponse(scopeStates.TryGetValue(subscriptionId, out var bucket) ? bucket : null);
            return true;
        }

        void SendResponse(Bucket? bucket)
        {
            var streamProcessorStateResponse = new StreamProcessorStateResponse
            {
                StreamKey = new StreamProcessorKey
                {
                    SubscriptionId = subscriptionId
                },
            };
            if(bucket is not null)
            {
                streamProcessorStateResponse.Bucket.Add(bucket);
            }
            respond(streamProcessorStateResponse);
        }
    }


    Task LoadScope(ScopeId scopeId)
    {
        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _loadingScopes.Add(scopeId, taskCompletionSource.Task);
        return LoadScopeInternal(scopeId, taskCompletionSource);
    }

    Task LoadScopeInternal(ScopeId scopeId, TaskCompletionSource taskCompletionSource)
    {
        var scopeTask = _repository.GetForScope(scopeId, Context.CancellationToken).ToListAsync().AsTask();
        Context.ReenterAfter(scopeTask, _ =>
        {
            if (scopeTask.IsCompletedSuccessfully)
            {
                var buckets = new Dictionary<StreamSubscriptionId, Bucket>();
                foreach (var streamProcessorStateWithId in scopeTask.Result)
                {
                    var streamProcessorKey = streamProcessorStateWithId.Id.ToProtobuf();
                    if (streamProcessorKey.IdCase == StreamProcessorKey.IdOneofCase.SubscriptionId)
                    {
                        buckets[streamProcessorKey.SubscriptionId] = streamProcessorStateWithId.State.ToProtobuf();
                    }
                }

                _subscriptionStates[scopeId] = buckets;
                taskCompletionSource.SetResult();
                _loadingScopes.Remove(scopeId);
            }
            else
            {
                _logger.LogError(scopeTask.Exception, "Failed to load scope {ScopeId}", scopeId);
                if (!Context.CancellationToken.IsCancellationRequested && !_shuttingDown)
                {
                    LoadScopeInternal(scopeId, taskCompletionSource);
                }
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

        if (!TrySetSubscriptionState(scopeId, subscriptionId, request.Bucket))
        {
            // No change, so no update
            return Task.CompletedTask;
        }

        AddChange(scopeId, subscriptionId, request.Bucket);

        PersistCurrentSubscriptionState(scopeId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update current in-memory state
    /// </summary>
    /// <returns>true if there were real changes, false otherwise</returns>
    bool TrySetSubscriptionState(ScopeId scope, StreamSubscriptionId subscriptionId, Bucket state)
    {
        if (!_subscriptionStates.TryGetValue(scope, out var scopedSubscriptionStates))
        {
            _subscriptionStates.Add(scope, scopedSubscriptionStates = new());
        }

        if (scopedSubscriptionStates.TryGetValue(subscriptionId, out var existing))
        {
            if (existing.Equals(state))
            {
                return false; // No change
            }
        }

        scopedSubscriptionStates[subscriptionId] = state;
        return true;
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
        _logger.LogTrace("Persisting {Count} changes for scope {ScopeId}", changes.Count, scopeId);
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
