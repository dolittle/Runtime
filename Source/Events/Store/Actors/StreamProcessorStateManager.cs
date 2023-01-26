// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(StreamProcessorStateActor), typeof(StreamProcessorStateClient))]
public class StreamProcessorStateManager : StreamProcessorStateBase
{
    readonly Dictionary<StreamProcessorKey, Bucket> _processorStates = new();
    Dictionary<StreamProcessorKey, Bucket> _currentChanges = new();
    readonly Dictionary<StreamSubscriptionId, Task<Bucket>> _waitingForSubscriptionStates = new();

    readonly IStreamProcessorStateBatchRepository _repository;


    public StreamProcessorStateManager(IContext context, IStreamProcessorStateBatchRepository repository) : base(context)
    {
        _repository = repository;
    }

    public override async Task OnStarted()
    {
        await foreach (var (id, state) in _repository.GetAllNonScoped(Context.CancellationToken))
        {
            _processorStates.Add(id.ToProtobuf(), state.ToProtobuf());
        }
    }

    public override Task Get(StreamProcessorKey key, Action<StreamProcessorStateResponse> respond, Action<string> onError)
    {
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
        var bucketTask = taskCompletionSource.Task;
        var task = _repository.TryGet(SubscriptionId.FromProtobuf(subscriptionId), Context.CancellationToken);
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
        return bucketTask;
    }

    public override Task SetStreamProcessorPartitionState(SetStreamProcessorPartitionStateRequest request)
    {
        _processorStates[request.StreamKey] = request.Bucket;
        _currentChanges[request.StreamKey] = request.Bucket;
        PersistCurrentState();
        return Task.CompletedTask;
    }

    void PersistCurrentState()
    {
        if (_currentChanges.Count == 0) return;
        var currentChanges = _currentChanges;
        _currentChanges = new Dictionary<StreamProcessorKey, Bucket>();
        Persist(currentChanges);
    }

    void Persist(Dictionary<StreamProcessorKey, Bucket> changes)
    {
        // throw new NotImplementedException();
        
        
        var streamProcessorStates = changes.ToDictionary(_ => _.Key.FromProtobuf(), _ => _.Value.FromProtobuf());
        var persistTask = _repository.Persist(streamProcessorStates, Context.CancellationToken);
        //
        // Context.ReenterAfter(persistTask, _ =>
        // {
        //     if (persistTask.IsCompletedSuccessfully)
        //     {
        //         PersistCurrentState();
        //     }
        // });
    }

    #region Unused, handled with reentrant overloads

    public override Task<StreamProcessorStateResponse> Get(StreamProcessorKey key) => throw new NotImplementedException("Not used");

    #endregion
}
