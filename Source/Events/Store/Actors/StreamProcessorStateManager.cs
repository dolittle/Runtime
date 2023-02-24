// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamProcessorStateActor), typeof(StreamProcessorStateClient))]
public class StreamProcessorStateManager : StreamProcessorStateBase
{
    readonly IStreamProcessorStateRepository _repository;
    readonly ILogger<StreamProcessorStateManager> _logger;
    readonly IApplicationLifecycleHooks _lifecycleHooks;

    readonly Dictionary<ScopeId, Dictionary<StreamProcessorId, Bucket>> _processorStates = new();
    readonly Dictionary<ScopeId, Dictionary<StreamProcessorId, Bucket>> _changedSubscriptionStates = new();

    readonly Dictionary<ScopeId, Task> _loadingScopes = new();

    readonly HashSet<ScopeId> _activeRequests = new();

    IShutdownHook? _shutdownHook;
    bool _shuttingDown;


    public StreamProcessorStateManager(IContext context, IStreamProcessorStateRepository repository, ILogger<StreamProcessorStateManager> logger,
        IApplicationLifecycleHooks lifecycleHooks) : base(context)
    {
        _repository = repository;
        _logger = logger;
        _lifecycleHooks = lifecycleHooks;
    }

    public override async Task OnStarted()
    {
        var defaultStates = new Dictionary<StreamProcessorId, Bucket>();
        await foreach (var (id, state) in _repository.GetNonScoped(Context.CancellationToken))
        {
            var streamProcessorKey = id.ToProtobuf();
            if (streamProcessorKey.IdCase != StreamProcessorKey.IdOneofCase.StreamProcessorId)
            {
                _logger.LogWarning("Got a non-stream processor key from the repository: {Key}", streamProcessorKey);
                continue;
            }

            defaultStates.Add(streamProcessorKey.StreamProcessorId, state.ToProtobuf());
        }
        _processorStates.Add(ScopeId.Default, defaultStates);

        _logger.LogInformation("Retrieved the state of {Count} stream processors", defaultStates.Count);

        _shutdownHook = _lifecycleHooks.RegisterShutdownHook();
        Context.ReenterAfter(_shutdownHook.ShuttingDown, () =>
        {
            _shuttingDown = true;
            if (_activeRequests.Count == 0) // No current changes
            {
                _shutdownHook.MarkCompleted();
            }
        });
    }


    public override Task GetByProcessorId(StreamProcessorId processorId, Action<StreamProcessorStateResponse> respond, Action<string> onError)
    {
        if (_shuttingDown)
        {
            onError("Runtime is shutting down");
            return Task.CompletedTask;
        }

        ScopeId scopeId = processorId.ScopeId.ToGuid();
        if (TrySendResponse(scopeId, processorId, respond))
        {
            return Task.CompletedTask;
        }

        if (!_loadingScopes.TryGetValue(scopeId, out var scopeHasLoadedTask))
        {
            scopeHasLoadedTask = LoadScope(scopeId);
        }


        Context.ReenterAfter(scopeHasLoadedTask, _ =>
        {
            if (_.IsCompletedSuccessfully && TrySendResponse(scopeId, processorId, respond))
            {
                // All OK
            }
            else
            {
                onError("Failed to get subscription state: " + _.Exception?.Message);
            }
        });
        return Task.CompletedTask;
    }

    bool TrySendResponse(ScopeId scopeId, StreamProcessorId processorId, Action<StreamProcessorStateResponse> respond)
    {
        {
            if (!_processorStates.TryGetValue(scopeId, out var scopeStates))
            {
                // Scope has not been loaded
                return false;
            }

            // The scope is already loaded
            SendResponse(scopeStates.TryGetValue(processorId, out var bucket) ? bucket : null);
            return true;
        }

        void SendResponse(Bucket? bucket)
        {
            var streamProcessorStateResponse = new StreamProcessorStateResponse
            {
                StreamKey = new StreamProcessorKey
                {
                    StreamProcessorId = processorId
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
                var buckets = new Dictionary<StreamProcessorId, Bucket>();
                foreach (var streamProcessorStateWithId in scopeTask.Result)
                {
                    var streamProcessorKey = streamProcessorStateWithId.Id.ToProtobuf();
                    if (streamProcessorKey.IdCase == StreamProcessorKey.IdOneofCase.StreamProcessorId)
                    {
                        buckets[streamProcessorKey.StreamProcessorId] = streamProcessorStateWithId.State.ToProtobuf();
                    }
                }

                _processorStates[scopeId] = buckets;
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


    public override Task SetByProcessorId(SetStreamProcessorStateRequest request)
    {
        if (request.StreamKey.IdCase != StreamProcessorKey.IdOneofCase.StreamProcessorId)
        {
            throw new ArgumentException("Can only set processor state by processor id");
        }

        var key = request.StreamKey.StreamProcessorId;
        ScopeId scopeId = key.ScopeId.ToGuid();

        if (!TrySetProcessorState(scopeId, key, request.Bucket))
        {
            // No change, so no update
            return Task.CompletedTask;
        }

        AddChange(scopeId, key, request.Bucket);
        PersistCurrentProcessorState(scopeId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update current in-memory state
    /// </summary>
    /// <returns>true if there were real changes, false otherwise</returns>
    bool TrySetProcessorState(ScopeId scope, StreamProcessorId processorId, Bucket state)
    {
        if (!_processorStates.TryGetValue(scope, out var scopedSubscriptionStates))
        {
            _processorStates.Add(scope, scopedSubscriptionStates = new());
        }

        if (scopedSubscriptionStates.TryGetValue(processorId, out var existing))
        {
            if (existing.Equals(state))
            {
                return false; // No change
            }
        }

        scopedSubscriptionStates[processorId] = state;
        return true;
    }

    void AddChange(ScopeId scope, StreamProcessorId id, Bucket state)
    {
        if (!_changedSubscriptionStates.TryGetValue(scope, out var changedScopedSubscriptionStates))
        {
            _changedSubscriptionStates.Add(scope, changedScopedSubscriptionStates = new());
        }

        changedScopedSubscriptionStates[id] = state;
    }

    void PersistCurrentProcessorState(ScopeId scopeId)
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

    void Persist(IReadOnlyDictionary<Processing.Streams.StreamProcessorId, IStreamProcessorState> changes, ScopeId scopeId)
    {
        _logger.LogTrace("Persisting {Count} changes for scope {ScopeId}", changes.Count, scopeId);
        Context.ReenterAfter(_repository.PersistForScope(scopeId, changes, Context.CancellationToken), _ =>
        {
            if (_.IsCompletedSuccessfully)
            {
                _activeRequests.Remove(scopeId);
                PersistCurrentProcessorState(scopeId);
                if (_shuttingDown && _activeRequests.Count == 0)
                {
                    _shutdownHook.MarkCompleted();
                }
            }
            else
            {
                _logger.FailedToPersistStreamSubscriptionState(_.Exception!, scopeId);
                // Try again
                Persist(changes, scopeId);
            }
        });
    }

    IReadOnlyDictionary<Dolittle.Runtime.Events.Processing.Streams.StreamProcessorId, IStreamProcessorState> FromProtobuf(
        IDictionary<StreamProcessorId, Bucket> changes)
    {
        var dict = new Dictionary<Processing.Streams.StreamProcessorId, IStreamProcessorState>();
        foreach (var change in changes)
        {
            var streamProcessorId = Processing.Streams.StreamProcessorId.FromProtobuf(change.Key);
            var state = change.Value.FromProtobuf();
            dict.Add(streamProcessorId, state);
        }

        return dict;
    }

    #region Unused

    public override Task<StreamProcessorStateResponse> GetByProcessorId(StreamProcessorId processorId) => throw new NotImplementedException("unused");

    #endregion
}
