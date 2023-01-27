﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
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


    readonly Dictionary<StreamProcessorId, Bucket> _processorStates = new();
    Dictionary<StreamProcessorId, Bucket> _changedProcessorStates = new();

    bool _activeRequest = false;
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
            if (!_activeRequest) // No current changes
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

    void PersistCurrentState()
    {
        if (_activeRequest)
        {
            return;
        }

        if (_changedProcessorStates.Count == 0) return;
        var currentChanges = _changedProcessorStates;
        _changedProcessorStates = new Dictionary<StreamProcessorId, Bucket>();


        _activeRequest = true;
        Persist(FromProtobuf(currentChanges));
    }

    static Dictionary<Processing.Streams.StreamProcessorId, IStreamProcessorState> FromProtobuf(Dictionary<StreamProcessorId, Bucket> currentChanges) =>
        currentChanges.ToDictionary(
            _ => Processing.Streams.StreamProcessorId.FromProtobuf(_.Key),
            _ => _.Value.FromProtobuf());

    void Persist(Dictionary<Processing.Streams.StreamProcessorId, IStreamProcessorState> changes)
    {

        var persistTask = _repository.PersistForScope(ScopeId.Default, changes, Context.CancellationToken);
        Context.ReenterAfter(persistTask, _ =>
        {
            if (persistTask.IsCompletedSuccessfully)
            {
                _activeRequest = false;
                PersistCurrentState();
                if (_shuttingDown && !_activeRequest)
                {
                    _shutdownHook.MarkCompleted();
                }
            }
            else
            {
                _logger.FailedToPersistStreamProcessorState(persistTask.Exception!);
                // Try again
                Persist(changes);
            }
        });
    }

}
