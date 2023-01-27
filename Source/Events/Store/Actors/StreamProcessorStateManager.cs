// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamProcessorStateActor), typeof(StreamProcessorStateClient))]
public class StreamProcessorStateManager : StreamProcessorStateBase
{
    readonly IStreamProcessorStateBatchRepository _repository;
    readonly ILogger<StreamProcessorStateManager> _logger;
    readonly IApplicationLifecycleHooks _lifecycleHooks;


    readonly Dictionary<StreamProcessorId, Bucket> _processorStates = new();
    readonly Dictionary<StreamProcessorId, Bucket> _changedProcessorStates = new();
    
    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _subscriptionStates = new();
    readonly Dictionary<ScopeId, Dictionary<StreamSubscriptionId, Bucket>> _changedSubscriptionStates = new();
    
    readonly HashSet<ScopeId> _activeRequests = new();
    readonly Dictionary<StreamSubscriptionId, Task<Bucket>> _waitingForSubscriptionStates = new();


    IShutdownHook _shutdownHook;
    bool _shuttingDown;


    public StreamProcessorStateManager(IContext context, IStreamProcessorStateBatchRepository repository, ILogger<StreamProcessorStateManager> logger,
        IApplicationLifecycleHooks lifecycleHooks) : base(context)
    {
        _repository = repository;
        _logger = logger;
        _lifecycleHooks = lifecycleHooks;
    }

    public override async Task OnStarted()
    {
        await foreach (var (id, state) in _repository.GetAllNonScoped(Context.CancellationToken))
        {
            var streamProcessorKey = id.ToProtobuf();
            if (streamProcessorKey.IdCase != StreamProcessorKey.IdOneofCase.StreamProcessorId)
            {
                _logger.LogWarning("Got a non-stream processor key from the repository: {Key}", streamProcessorKey);
                continue;
            }

            _processorStates.Add(streamProcessorKey.StreamProcessorId, state.ToProtobuf());
        }

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

    public override Task<StreamProcessorStateResponse> GetByProcessorId(StreamProcessorId requestId)
    {
        if (_shuttingDown)
        {
            throw new RuntimeShuttingDown();
        }

        var response = new StreamProcessorStateResponse
        {
            StreamKey = new StreamProcessorKey { StreamProcessorId = requestId },
        };

        if (_processorStates.TryGetValue(requestId, out var existingBucket))
        {
            response.Bucket.Add(existingBucket);
        }

        return Task.FromResult(response);
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

    public override Task SetByProcessorId(SetStreamProcessorStateRequest request)
    {
        if (request.StreamKey.IdCase != StreamProcessorKey.IdOneofCase.StreamProcessorId)
        {
            throw new ArgumentException("Can only set processor state by processor id");
        }

        var streamProcessorId = request.StreamKey.StreamProcessorId;
        _processorStates[streamProcessorId] = request.Bucket;
        _changedProcessorStates[streamProcessorId] = request.Bucket;
        return Task.CompletedTask;
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
        // Persist(currentChanges, scopeId);
    }

    // void Persist(Dictionary<SubscriptionId, Bucket> changes, ScopeId scopeId)
    // {
    //     var streamProcessorStates = changes.ToDictionary(_ => _.Key.FromProtobuf(), _ => _.Value.FromProtobuf());
    //     var persistTask = _repository.Persist(streamProcessorStates, Context.CancellationToken);
    //     Context.ReenterAfter(persistTask, _ =>
    //     {
    //         if (persistTask.IsCompletedSuccessfully)
    //         {
    //             _activeRequests.Remove(scopeId);
    //             PersistCurrentSubscriptionState(scopeId);
    //             if (_shuttingDown && _activeRequests.Count == 0)
    //             {
    //                 _shutdownHook.MarkCompleted();
    //             }
    //         }
    //         else
    //         {
    //             _logger.FailedToPersistStreamProcessorState(persistTask.Exception!, scopeId);
    //             // Try again
    //             Persist(changes, scopeId);
    //         }
    //     });
    // }

    #region Unused, handled with reentrant overloads

    public override Task<StreamProcessorStateResponse> GetBySubscriptionId(StreamSubscriptionId request) => throw new NotImplementedException("unused");

    #endregion
}
