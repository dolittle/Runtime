// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Google.Protobuf.WellKnownTypes;
using Integration.Shared;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using CommittedEvent = Dolittle.Runtime.Events.Store.CommittedEvent;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
using StreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;
using MongoStreamEvent = Dolittle.Runtime.Events.Store.MongoDB.Events.StreamEvent;

namespace Integration.Tests.Events.Processing.EventHandlers.given;

class single_tenant_and_event_handlers : Processing.given.a_clean_event_store
{
    protected const int number_of_event_types = 2;
    protected static IEventHandlers event_handlers;
    protected static IEventHandlerFactory event_handler_factory;
    protected static IEnumerable<IEventHandler> event_handlers_to_run;
    protected static ArtifactId[] event_types;
    protected static Mock<ReverseCallDispatcher> dispatcher;
    protected static TaskCompletionSource dispatcher_cancellation_source;
    protected static CommittedEvents committed_events;
    protected static Dictionary<ScopeId, CommittedEvents> scoped_committed_events;
    protected static IWriteEventHorizonEvents event_horizon_events_writer;
    static int number_of_events_handled;
    static CancellationTokenRegistration? cancel_event_handlers_registration;
    static CancellationTokenSource? cancel_event_handlers_source;

    static Dictionary<EventHandlerInfo, Try<IStreamDefinition>> persisted_stream_definitions;
    static Dictionary<StreamProcessorId, Try<IStreamProcessorState>> persisted_stream_processor_states;

    Establish context = () =>
    {
        number_of_events_handled = 0;
        scoped_committed_events = new Dictionary<ScopeId, CommittedEvents>();
        committed_events = new CommittedEvents(Array.Empty<CommittedEvent>());
        dispatcher_cancellation_source = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        event_horizon_events_writer = runtime.Host.Services.GetRequiredService<Func<TenantId, IWriteEventHorizonEvents>>()(tenant);
        event_handlers = runtime.Host.Services.GetRequiredService<IEventHandlers>();
        event_handler_factory = runtime.Host.Services.GetRequiredService<IEventHandlerFactory>();
        event_types = Enumerable.Range(0, number_of_event_types).Select(_ => new ArtifactId(Guid.NewGuid())).ToArray();
        dispatcher = new Mock<ReverseCallDispatcher>();
        setup_dispatcher_call(_ => new EventHandlerResponse());

        dispatcher
            .Setup(_ => _.Reject(Moq.It.IsAny<EventHandlerRegistrationResponse>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        dispatcher
            .Setup(_ => _.Accept(Moq.It.IsAny<EventHandlerRegistrationResponse>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(dispatcher_cancellation_source.Task);
    };

    Cleanup clean = () =>
    {
        cancel_event_handlers_registration?.Dispose();
        cancel_event_handlers_source?.Dispose();
        foreach (var event_handler in event_handlers_to_run)
        {
            event_handler.Dispose();
        }
    };

    static async Task commit_events(UncommittedEvents uncommitted_events)
    {
        var response = await event_store.Commit(uncommitted_events, Runtime.CreateExecutionContextFor(tenant)).ConfigureAwait(false);
        var newly_committed_events = response.Events.ToCommittedEvents();
        var all_committed_events = committed_events.ToList();
        all_committed_events.AddRange(newly_committed_events);
        committed_events = new CommittedEvents(all_committed_events);

    }

    protected static IEnumerable<Dolittle.Runtime.Events.Store.Streams.StreamEvent> get_partitioned_events_in_stream(IEventHandler event_handler, PartitionId partition_id)
    {
        var fetcher = event_fetchers.GetPartitionedFetcherFor(
            event_handler.Info.Id.Scope,
            new StreamDefinition(new TypeFilterWithEventSourcePartitionDefinition(
                StreamId.EventLog,
                event_handler.Info.Id.EventHandler.Value,
                event_handler.Info.EventTypes,
                event_handler.Info.Partitioned)), CancellationToken.None).Result;

        return fetcher.FetchInPartition(partition_id, StreamPosition.Start, CancellationToken.None).Result.Result;
    }

    protected static async Task commit_events_for_each_event_type(IEnumerable<(int number_of_events, EventSourceId event_source, ScopeId scope_id)> commit)
    {
        IEnumerable<UncommittedEvent> CreateUncommittedEvents(IEnumerable<(int number_of_events, EventSourceId event_source)> commit)
        {
            return event_types.SelectMany(event_type => commit.SelectMany(tuple =>
            {
                var (number_of_events, event_source) = tuple;

                return Enumerable.Range(0, number_of_events)
                    .Select(_ => new UncommittedEvent(event_source, new Artifact(event_type, ArtifactGeneration.First), false, "{\"hello\": 42}"));
            }));
        }

        var unscoped_commmits = commit.Where(_ => _.scope_id == ScopeId.Default).Select(_ => (_.number_of_events, _.event_source));
        var tasks = new List<Task>();
        if (unscoped_commmits.Any())
        {
            var unscoped_uncommitted_events = new UncommittedEvents(
                CreateUncommittedEvents(commit.Where(_ => _.scope_id == ScopeId.Default).Select(_ => (_.number_of_events, _.event_source))).ToArray());
            tasks.Add(commit_events(unscoped_uncommitted_events));
        }
        
        var scoped_uncommitted_events = new Dictionary<ScopeId, UncommittedEvents>();
        foreach (var scoped_commit_group in commit.Where(_ => _.scope_id != ScopeId.Default).GroupBy(_ => _.scope_id))
        {
            var uncommitted_events = CreateUncommittedEvents(scoped_commit_group.Select(_ => (_.number_of_events, _.event_source)));
            scoped_uncommitted_events[scoped_commit_group.Key] = new UncommittedEvents(uncommitted_events.ToArray());
        }


        foreach (var (scope, uncommittedEvents) in scoped_uncommitted_events.Where(_ => _.Value.HasEvents))
        {
            var committedEvents = new CommittedEvents(uncommittedEvents.Select(_ => new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.UtcNow,
                _.EventSource,
                Runtime.CreateExecutionContextFor(tenant),
                _.Type,
                _.Public,
                _.Content)).ToList());
            tasks.Add(write_committed_events_to_scoped_event_log(committedEvents, scope));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    static async Task write_committed_events_to_scoped_event_log(CommittedEvents committed_events, ScopeId scope)
    {
        foreach (var committed_event in committed_events)
        {
            await event_horizon_events_writer.Write(committed_event, Guid.NewGuid(), scope, CancellationToken.None).ConfigureAwait(false);
        }
        if (!scoped_committed_events.TryGetValue(scope, out var previously_committed_events))
        {
            previously_committed_events = new CommittedEvents(Array.Empty<CommittedEvent>());
        }
        var all_committed_events = previously_committed_events.ToList();
        all_committed_events.AddRange(committed_events);
        scoped_committed_events[scope] = new CommittedEvents(all_committed_events);
    }

    protected static async Task run_event_handlers_until_completion(IEnumerable<Task> pre_start_tasks = default, Task post_start_task = default)
    {
        await Task.WhenAll(pre_start_tasks ?? Array.Empty<Task>()).ConfigureAwait(false);
        var tasks = new List<Task>();
        tasks.AddRange(event_handlers_to_run.Select(event_handler => event_handlers.RegisterAndStart(
            event_handler,
            (_, _) => dispatcher.Object.Reject(new EventHandlerRegistrationResponse(), CancellationToken.None),
            CancellationToken.None)));
        tasks.Add(post_start_task ?? Task.CompletedTask);

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    protected static Task run_event_handlers_until_completion_and_commit_events_after_starting_event_handlers(params (int number_of_events, EventSourceId event_source, ScopeId scope)[] events)
        => run_event_handlers_until_completion(post_start_task: commit_after_delay(events));

    protected static void with_event_handlers(params (bool partitioned, int max_event_types_to_filter, ScopeId scope, bool fast, bool implicitFilter)[] event_handler_infos)
    {
        event_handlers_to_run = event_handler_infos.Select(_ =>
        {
            var (partitioned, max_event_types_to_filter, scope, fast, implicitFilter) = _;
            var registration_arguments = new EventHandlerRegistrationArguments(
                Runtime.CreateExecutionContextFor(tenant), Guid.NewGuid(), event_types.Take(max_event_types_to_filter), partitioned, scope);
            return fast
                ? event_handler_factory.CreateFast(registration_arguments, implicitFilter, dispatcher.Object, CancellationToken.None)
                : event_handler_factory.Create(registration_arguments, dispatcher.Object, CancellationToken.None);
        }).ToArray();
    }

    static void setup_dispatcher_call(Func<HandleEventRequest, EventHandlerResponse> callback) 
    {
        dispatcher
            .Setup(_ => _.Call(Moq.It.IsAny<HandleEventRequest>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Callback(() => Interlocked.Add(ref number_of_events_handled, 1))
            .Returns<HandleEventRequest, ExecutionContext, CancellationToken>((request, _, _) =>
            {
                cancel_event_handlers_registration?.Unregister();
                cancel_event_handlers_registration?.Dispose();
                cancel_event_handlers_source?.Dispose();
                var response = callback(request);
                cancel_event_handlers_source = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
                cancel_event_handlers_registration = cancel_event_handlers_source.Token.Register(() => dispatcher_cancellation_source?.SetResult());
                return Task.FromResult(response);
            });
    }

    protected static void fail_after_processing_number_of_events(int number_of_events, string reason, retry_failing_event? retry = default)
    {
        setup_dispatcher_call(_ =>
        {
            var response = new EventHandlerResponse();
            if (number_of_events_handled >= number_of_events)
            {
                response.Failure = new ProcessorFailure
                {
                    Reason = reason, 
                    Retry = retry is not null,
                    RetryTimeout = retry?.timeout.ToDuration() ?? TimeSpan.FromSeconds(1).ToDuration()
                };
            }
            return response;
        });
    }

    protected static IEnumerable<CommittedEvent> committed_events_for_event_types(int num_event_types)
    {
        var nFirstEventTypes = event_types.Take(num_event_types);
        return committed_events.Where(_ => nFirstEventTypes.Contains(_.Type.Id));
    }
    
    protected static void fail_for_partitions(IEnumerable<PartitionId> partitions, string reason, retry_failing_event? retry = default)
    {
        setup_dispatcher_call(request =>
        {
            var response = new EventHandlerResponse();
            if (partitions.Select(_ => _.Value).Contains(request.Event.PartitionId))
            {
                response.Failure = new ProcessorFailure
                {
                    Reason = reason, 
                    Retry = retry is not null,
                    RetryTimeout = retry?.timeout.ToDuration() ?? TimeSpan.FromSeconds(1).ToDuration()
                };
            }
            return response;
        });
    }

    static async Task commit_after_delay(IEnumerable<(int number_of_events, EventSourceId event_source, ScopeId scope)> commit)
    {
        if (commit.Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            await commit_events_for_each_event_type(commit);
        }
    }

    protected static Try<IStreamDefinition> get_stream_definition_for(IEventHandler event_handler)
    {
        if (persisted_stream_definitions == default)
        {
            persisted_stream_definitions = new Dictionary<EventHandlerInfo, Try<IStreamDefinition>>();
            foreach (var info in event_handlers_to_run.Select(_ => _.Info))
            {
                var tryGetDefinition = stream_definition_repository.TryGet(info.Id.Scope, info.Id.EventHandler.Value, CancellationToken.None).Result;
                persisted_stream_definitions[info] = tryGetDefinition;
            }
        }
        if (!persisted_stream_definitions.ContainsKey(event_handler.Info))
        {
            var tryGetDefinition = stream_definition_repository.TryGet(event_handler.Info.Id.Scope, event_handler.Info.Id.EventHandler.Value, CancellationToken.None).Result;
            persisted_stream_definitions[event_handler.Info] = tryGetDefinition;
        }
        return persisted_stream_definitions[event_handler.Info];
    }
    protected static Try<IStreamProcessorState> get_stream_processor_state_for(StreamProcessorId stream_processor_id)
    {
        if (persisted_stream_processor_states == default)
        {
            persisted_stream_processor_states = new Dictionary<StreamProcessorId, Try<IStreamProcessorState>>();
            foreach (var info in event_handlers_to_run.Select(_ => _.Info))
            {
                var filterProcessorId = info.GetFilterStreamId();
                var eventProcessorId = info.GetEventProcessorStreamId();
                persisted_stream_processor_states[filterProcessorId] = stream_processor_states.TryGetFor(filterProcessorId,  CancellationToken.None).Result;;
                persisted_stream_processor_states[eventProcessorId] = stream_processor_states.TryGetFor(eventProcessorId,  CancellationToken.None).Result;;
            }
        }
        if (!persisted_stream_processor_states.ContainsKey(stream_processor_id))
        {
            persisted_stream_processor_states[stream_processor_id] = stream_processor_states.TryGetFor(stream_processor_id,  CancellationToken.None).Result;;
        }
        return persisted_stream_processor_states[stream_processor_id];
    }

    #region SpecHelpers

    protected static void expect_number_of_filtered_events(IEventHandler event_handler, long expected_number)
        => count_events_in_stream(event_handler, Builders<MongoStreamEvent>.Filter.Empty).ShouldEqual(expected_number);
    
    protected static void expect_number_of_filtered_events_with_partition(IEventHandler event_handler, long expected_number, PartitionId partition_id)
        => count_events_in_stream(event_handler, Builders<MongoStreamEvent>.Filter.Eq(_ => _.Partition, partition_id.Value)).ShouldEqual(expected_number);
    
    protected static void expect_number_of_filtered_events_with_event_type(IEventHandler event_handler, long expected_number, ArtifactId event_type)
        => count_events_in_stream(event_handler, Builders<MongoStreamEvent>.Filter.Eq(_ => _.Metadata.TypeId, event_type.Value)).ShouldEqual(expected_number);
    

    static long count_events_in_stream(IEventHandler event_handler, FilterDefinition<MongoStreamEvent> filter)
        => streams
            .Get(event_handler.Info.Id.Scope, event_handler.Info.Id.EventHandler.Value, CancellationToken.None)
            .Result
            .CountDocuments(filter, cancellationToken: CancellationToken.None);
    

    protected static void expect_stream_definition(
        IEventHandler event_handler,
        bool partitioned = false,
        bool public_stream = false,
        int? max_handled_event_types = 0)
    {
        var handledEventTypes = event_types.Take(max_handled_event_types ?? number_of_event_types);
        handledEventTypes ??= event_types;
        var tryGetStreamDefinition = get_stream_definition_for(event_handler);
        tryGetStreamDefinition.Success.ShouldBeTrue();
        var streamDefinition = tryGetStreamDefinition.Result;
        streamDefinition.Partitioned.ShouldEqual(partitioned);
        streamDefinition.Public.ShouldEqual(public_stream);
        streamDefinition.FilterDefinition.ShouldBeOfExactType<TypeFilterWithEventSourcePartitionDefinition>();
        var filterDefinition = streamDefinition.FilterDefinition as TypeFilterWithEventSourcePartitionDefinition;
        filterDefinition!.Partitioned.ShouldEqual(partitioned);
        filterDefinition!.Public.ShouldEqual(public_stream);
        filterDefinition!.SourceStream.ShouldEqual(StreamId.EventLog);
        filterDefinition!.TargetStream.ShouldEqual<StreamId>(event_handler.Info.Id.EventHandler.Value);
        filterDefinition!.Types.ShouldContainOnly(handledEventTypes);
    }

    protected static void expect_stream_processor_state(
        IEventHandler event_handler,
        bool implicit_filter = false,
        bool partitioned = false,
        int num_events_to_handle = 0,
        failing_partitioned_state? failing_partitioned_state = null,
        failing_unpartitioned_state? failing_unpartitioned_state = null
        )
    {
        expect_filter_stream_processor_state(event_handler.Info.GetFilterStreamId(), implicit_filter);
        expect_event_processor_stream_processor_state(event_handler.Info.GetEventProcessorStreamId(), partitioned, num_events_to_handle, failing_partitioned_state, failing_unpartitioned_state);
    }

    static void expect_filter_stream_processor_state(StreamProcessorId id, bool implicit_filter)
    {
        var tryGetStreamProcessorState = get_stream_processor_state_for(id);
        tryGetStreamProcessorState.Success.ShouldEqual(!implicit_filter);
        if (implicit_filter)
        {
            return;
        }
        var state = tryGetStreamProcessorState.Result;
        state.Partitioned.ShouldBeFalse();
        state.Position.Value.ShouldEqual((ulong) committed_events.Count);
        state.ShouldBeOfExactType<StreamProcessorState>();
        var unpartitionedState = state as StreamProcessorState;
        unpartitionedState!.IsFailing.ShouldBeFalse();
        unpartitionedState.ProcessingAttempts.ShouldEqual((uint)0);
        unpartitionedState.RetryTime.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
        unpartitionedState.LastSuccessfullyProcessed.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
    }

    static void expect_event_processor_stream_processor_state(
        StreamProcessorId id,
        bool partitioned,
        int num_events_to_handle,
        failing_partitioned_state? failing_partitioned_state,
        failing_unpartitioned_state? failing_unpartitioned_state
        )
    {
        var tryGetStreamProcessorState = get_stream_processor_state_for(id);
        tryGetStreamProcessorState.Success.ShouldBeTrue();
        var state = tryGetStreamProcessorState.Result;
        state.Partitioned.ShouldEqual(partitioned);
        
        if (partitioned)
        {
            state.Position.Value.ShouldEqual((ulong) num_events_to_handle);
            expect_partitioned_event_processor_stream_processor_state(state, failing_partitioned_state);
        }
        else
        {
            expect_unpartitioned_event_processor_stream_processor_state(state, num_events_to_handle, failing_unpartitioned_state);
        }
    }

    static void expect_partitioned_event_processor_stream_processor_state(
        IStreamProcessorState state,
        failing_partitioned_state? failing_partitioned_state)
    {
        state.ShouldBeOfExactType<Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState>();
        var partitionedState = state as Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState;
        
        if (failing_partitioned_state is null)
        {
            partitionedState!.FailingPartitions.ShouldBeEmpty();
            partitionedState.LastSuccessfullyProcessed.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
        }
        else
        {
            partitionedState!.FailingPartitions.Keys.ShouldContainOnly(failing_partitioned_state.failing_partitions.Keys);
            foreach (var (partition, failingState) in partitionedState!.FailingPartitions)
            {
                var failingPosition = failing_partitioned_state.failing_partitions[partition];
                failingState.Position.ShouldEqual(failingPosition);
                failingState.LastFailed.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
                failingState.ProcessingAttempts.ShouldBeGreaterThanOrEqualTo(1);
            }
        }
    }
    static void expect_unpartitioned_event_processor_stream_processor_state(
        IStreamProcessorState state,
        int num_events_to_handle,
        failing_unpartitioned_state? failing_unpartitioned_state)
    {
        state.ShouldBeOfExactType<StreamProcessorState>();
        var unpartitionedState = state as StreamProcessorState;
        
        if (failing_unpartitioned_state is null)
        {
            unpartitionedState!.IsFailing.ShouldBeFalse();
            unpartitionedState.ProcessingAttempts.ShouldEqual((uint)0);
            unpartitionedState.LastSuccessfullyProcessed.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
            unpartitionedState.RetryTime.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
            unpartitionedState.Position.Value.ShouldEqual((ulong) num_events_to_handle);
        }
        else
        {
            unpartitionedState!.IsFailing.ShouldBeTrue();
            unpartitionedState.Position.ShouldEqual(failing_unpartitioned_state.failing_position);
            unpartitionedState.ProcessingAttempts.ShouldBeGreaterThanOrEqualTo(1);
            unpartitionedState.LastSuccessfullyProcessed.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow);
        }
    }
}

record failing_partitioned_state(Dictionary<PartitionId, StreamPosition> failing_partitions);
record failing_unpartitioned_state(StreamPosition failing_position);

    #endregion

    record retry_failing_event(TimeSpan timeout);

    