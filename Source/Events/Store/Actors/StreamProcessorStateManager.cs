// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamProcessorStateActor), typeof(StreamProcessorStateClient))]
public class StreamProcessorStateManager : StreamProcessorStateBase
{
    readonly IStreamProcessorStateRepository _repository;
    readonly ILogger<StreamProcessorStateManager> _logger;
    readonly IApplicationLifecycleHooks _lifecycleHooks;


    readonly Dictionary<StreamProcessorKey, Bucket> _processorStates = new();

    // Dictionary<StreamProcessorKey, Bucket> _currentChanges = new();
    readonly Dictionary<ScopeId, Dictionary<StreamProcessorKey, Bucket>> _currentChangesByScope = new();
    readonly HashSet<ScopeId> _activeRequests = new();
    readonly Dictionary<StreamSubscriptionId, Task<Bucket>> _waitingForSubscriptionStates = new();


    IShutdownHook _shutdownHook;
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
        await foreach (var (id, state) in _repository.GetNonScoped(Context.CancellationToken))
        {
            _processorStates.Add(id.ToProtobuf(), state.ToProtobuf());
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

    public override Task Get(StreamProcessorKey key, Action<StreamProcessorStateResponse> respond, Action<string> onError)
    {
        if (_shuttingDown)
        {
            onError("Runtime is shutting down");
            return Task.CompletedTask;
        }

        if (_processorStates.TryGetValue(key, out var existingBucket))
        {
            respond(new StreamProcessorStateResponse
            {
                StreamKey = key,
                Bucket = { existingBucket }
            });
        }

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (key.IdCase)
        {
            case StreamProcessorKey.IdOneofCase.StreamProcessorId:
                respond(InitStreamProcessorState(key, respond));
                break;
            case StreamProcessorKey.IdOneofCase.SubscriptionId:
                GetSubscriptionState(key.SubscriptionId, respond, onError);
                break;
            default:
                onError("Invalid StreamProcessorKey");
                break;
        }

        return Task.CompletedTask;
    }

    StreamProcessorStateResponse InitStreamProcessorState(StreamProcessorKey key, Action<StreamProcessorStateResponse> respond)
    {
        var newBucket = new Bucket
        {
            BucketId = 0,
            CurrentOffset = 0
        };
        _processorStates.Add(key, newBucket);
        return new StreamProcessorStateResponse
        {
            StreamKey = key,
            Bucket = { newBucket }
        };
    }

    void GetSubscriptionState(StreamSubscriptionId subscriptionId, Action<StreamProcessorStateResponse> respond, Action<string> onError)
    {
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

    public override Task SetStreamProcessorPartitionState(SetStreamProcessorPartitionStateRequest request)
    {
        if (_shuttingDown)
        {
            throw new RuntimeShuttingDown();
        }

        var streamProcessorKey = request.StreamKey;
        var processorState = request.Bucket;
        _processorStates[streamProcessorKey] = processorState;
        var scopeId = streamProcessorKey.FromProtobuf().ScopeId;

        AddChange(scopeId, streamProcessorKey, processorState);
        PersistCurrentState(scopeId);
        return Task.CompletedTask;
    }

    void AddChange(ScopeId scopeId, StreamProcessorKey streamProcessorKey, Bucket processorState)
    {
        if (!_currentChangesByScope.TryGetValue(scopeId, out var currentChanges))
        {
            _currentChangesByScope[scopeId] = currentChanges = new Dictionary<StreamProcessorKey, Bucket>();
        }

        currentChanges[streamProcessorKey] = processorState;
    }

    void PersistCurrentState(ScopeId scopeId)
    {
        if (_activeRequests.Contains(scopeId))
        {
            return;
        }

        if (_currentChangesByScope.Count == 0) return;
        if (!_currentChangesByScope.Remove(scopeId, out var currentChanges))
        {
            return; // No changes for this scope
        }

        _activeRequests.Add(scopeId);
        Persist(currentChanges, scopeId);
    }

    void Persist(Dictionary<StreamProcessorKey, Bucket> changes, ScopeId scopeId)
    {
        var streamProcessorStates = changes.ToDictionary(_ => _.Key.FromProtobuf(), _ => _.Value.FromProtobuf());
        var persistTask = _repository.PersistForScope(streamProcessorStates, Context.CancellationToken);
        Context.ReenterAfter(persistTask, _ =>
        {
            if (persistTask.IsCompletedSuccessfully)
            {
                _activeRequests.Remove(scopeId);
                PersistCurrentState(scopeId);
                if (_shuttingDown && _activeRequests.Count == 0)
                {
                    _shutdownHook.MarkCompleted();
                }
            }
            else
            {
                _logger.FailedToPersistStreamProcessorState(persistTask.Exception!, scopeId);
                // Try again
                Persist(changes, scopeId);
            }
        });
    }

    #region Unused, handled with reentrant overloads

    public override Task<StreamProcessorStateResponse> Get(StreamProcessorKey key) => throw new NotImplementedException("Not used");

    #endregion
}
