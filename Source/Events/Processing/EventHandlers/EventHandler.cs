// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Runtime.ExceptionServices;
// using System.Threading;
// using System.Threading.Tasks;
// using Dolittle.Runtime.Artifacts;
// using Dolittle.Runtime.Domain.Tenancy;
// using Dolittle.Runtime.Events.Processing.Filters;
// using Dolittle.Runtime.Events.Processing.Streams;
// using Dolittle.Runtime.Events.Store;
// using Dolittle.Runtime.Events.Store.Streams;
// using Dolittle.Runtime.Events.Store.Streams.Filters;
// using Dolittle.Runtime.Protobuf;
// using Dolittle.Runtime.Rudimentary;
// using Microsoft.Extensions.Logging;
// using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
//
// namespace Dolittle.Runtime.Events.Processing.EventHandlers;
//
// /// <summary>
// /// Represents an event handler in the system.
// /// </summary>
// /// <remarks>
// /// An event handler is a formalized type that consists of a filter and an event processor.
// /// The filter filters off of an event log based on the types of events the handler is interested in
// /// and puts these into a stream for the filter. From this new stream, the event processor will handle
// /// the forwarding to the client for it to handle the event.
// /// What sets an event handler apart is that it has a formalization around the stream definition that
// /// consists of the events it is interested in, which is defined from the client.
// /// </remarks>
// public class EventHandler : IEventHandler
// {
//     readonly IStreamProcessors _streamProcessors;
//     readonly IValidateFilterForAllTenants _filterValidator;
//     readonly IStreamDefinitions _streamDefinitions;
//     readonly EventHandlerRegistrationArguments _arguments;
//     readonly Func<TenantId, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> _filterProcessorForTenant;
//     readonly Func<TenantId, IEventProcessor> _eventProcessorForTenant;
//     readonly Func<CancellationToken, Task> _acceptRegistration;
//     readonly Func<Failure, CancellationToken, Task> _rejectRegistration;
//     readonly IMetricsCollector _metrics;
//     readonly ILogger _logger;
//     readonly ExecutionContext _executionContext;
//     readonly CancellationTokenSource _cancellationTokenSource;
//
//     bool _disposed;
//
//     /// <summary>
//     /// Initializes a new instance of <see cref="EventHandler"/>.
//     /// </summary>
//     /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
//     /// <param name="filterValidationForAllTenants">The <see cref="IValidateFilterForAllTenants" /> for validating the filter definition.</param>
//     /// <param name="streamDefinitions">The<see cref="IStreamDefinitions" />.</param>
//     /// <param name="arguments">Connecting arguments.</param>
//     /// <param name="filterProcessorForTenant"></param>
//     /// <param name="eventProcessorForTenant">The event processor.</param>
//     /// <param name="acceptRegistration">Accepts the event handler registration.</param>
//     /// <param name="rejectRegistration">Rejects the event handler registration.</param>
//     /// <param name="metrics">The collector to use for metrics.</param>
//     /// <param name="logger">Logger for logging.</param>
//     /// <param name="executionContext">The execution context for the event handler.</param>
//     /// <param name="cancellationToken">Cancellation token that can cancel the hierarchy.</param>
//     public EventHandler(
//         IStreamProcessors streamProcessors,
//         IValidateFilterForAllTenants filterValidationForAllTenants,
//         IStreamDefinitions streamDefinitions,
//         EventHandlerRegistrationArguments arguments,
//         Func<TenantId, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> filterProcessorForTenant,
//         Func<TenantId, IEventProcessor> eventProcessorForTenant,
//         Func<CancellationToken, Task> acceptRegistration,
//         Func<Failure, CancellationToken, Task> rejectRegistration,
//         IMetricsCollector metrics,
//         ILogger logger,
//         ExecutionContext executionContext,
//         CancellationToken cancellationToken)
//     {
//         _logger = logger;
//         _streamProcessors = streamProcessors;
//         _filterValidator = filterValidationForAllTenants;
//         _streamDefinitions = streamDefinitions;
//         _arguments = arguments;
//         _filterProcessorForTenant = filterProcessorForTenant;
//         _executionContext = executionContext;
//         _eventProcessorForTenant = eventProcessorForTenant;
//         _acceptRegistration = acceptRegistration;
//         _rejectRegistration = rejectRegistration;
//         _metrics = metrics;
//         _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
//     }
//     
//     StreamId TargetStream => _arguments.EventHandler.Value;
//
//     /// <inheritdoc />
//     public EventHandlerInfo Info => new(
//         new EventHandlerId(_arguments.Scope, _arguments.EventHandler),
//         _arguments.HasAlias,
//         _arguments.Alias,
//         _arguments.EventTypes,
//         _arguments.Partitioned);
//     
//     public ScopeId Scope => _arguments.Scope.Value;
//     
//     public EventProcessorId EventProcessor => _arguments.EventHandler.Value;
//     
//     public IEnumerable<ArtifactId> EventTypes => _arguments.EventTypes;
//     
//     public bool Partitioned => _arguments.Partitioned;
//     
//     public StreamDefinition FilteredStreamDefinition => new(
//         new TypeFilterWithEventSourcePartitionDefinition(
//             TargetStream,
//             TargetStream,
//             EventTypes,
//             Partitioned));
//
//     public TypeFilterWithEventSourcePartitionDefinition FilterDefinition => new(
//         StreamId.EventLog,
//         TargetStream,
//         EventTypes,
//         Partitioned);
//
//     StreamProcessor FilterStreamProcessor { get; set; }
//
//     StreamProcessor EventProcessorStreamProcessor { get; set; }
//
//
//     /// <inheritdoc />
//     public Task<Try<ProcessingPosition>> ReprocessEventsFrom(TenantId tenant, ProcessingPosition position)
//         => EventProcessorStreamProcessor.SetToPosition(tenant, position);
//
//     /// <inheritdoc />
//     public async Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents()
//     {
//         try
//         {
//             return Try<IDictionary<TenantId, Try<ProcessingPosition>>>.Succeeded(await EventProcessorStreamProcessor.SetToInitialPositionForAllTenants().ConfigureAwait(false));
//         }
//         catch (Exception ex)
//         {
//             return ex;
//         }
//     }
//
//     /// <inheritdoc/>
//     public void Dispose()
//     {
//         Dispose(true);
//         GC.SuppressFinalize(this);
//     }
//
//     /// <summary>
//     /// Dispose managed and unmanaged resources.
//     /// </summary>
//     /// <param name="disposing">Whether to dispose managed resources.</param>
//     protected virtual void Dispose(bool disposing)
//     {
//         if (_disposed)
//         {
//             return;
//         }
//         if (disposing)
//         {
//             FilterStreamProcessor?.Dispose();
//             EventProcessorStreamProcessor?.Dispose();
//             _cancellationTokenSource.Dispose();
//         }
//
//         _disposed = true;
//     }
//
//     /// <inheritdoc />
//     public async Task RegisterAndStart()
//     {
//         _logger.ConnectingEventHandlerWithId(EventProcessor);
//         if (await RejectIfNonWriteableStream().ConfigureAwait(false)
//             || !await RegisterFilterStreamProcessor().ConfigureAwait(false)
//             || !await RegisterEventProcessorStreamProcessor().ConfigureAwait(false))
//         {
//             return;
//         }
//
//         await Start().ConfigureAwait(false);
//     }
//
//     /// <inheritdoc />
//     public event EventHandlerRegistrationFailed? OnRegistrationFailed;
//
//     async Task<bool> RejectIfNonWriteableStream()
//     {
//         if (!TargetStream.IsNonWriteable)
//         {
//             return false;
//         }
//         _logger.EventHandlerIsInvalid(EventProcessor);
//         await Fail(
//             EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
//             $"Cannot register Event Handler: '{EventProcessor.Value}' because it is an invalid Stream Id").ConfigureAwait(false);
//
//         return true;
//     }
//
//     async Task<bool> RegisterFilterStreamProcessor()
//     {
//         _logger.RegisteringStreamProcessorForFilter(EventProcessor);
//         return await RegisterStreamProcessor(
//             "EventHandler-Filter",
//             new EventLogStreamDefinition(),
//             _filterProcessorForTenant,
//             HandleFailedToRegisterFilter,
//             (streamProcessor) => FilterStreamProcessor = streamProcessor
//         ).ConfigureAwait(false);
//     }
//
//     Failure HandleFailedToRegisterFilter(Exception exception)
//     {
//         _logger.ErrorWhileRegisteringStreamProcessorForFilter(exception, EventProcessor);
//
//         if (exception is StreamProcessorAlreadyRegistered)
//         {
//             _logger.EventHandlerAlreadyRegistered(EventProcessor);
//             return new Failure(
//                 EventHandlersFailures.FailedToRegisterEventHandler,
//                 $"Failed to register Event Handler: {EventProcessor.Value}. Filter already registered");
//         }
//         return new Failure(
//             EventHandlersFailures.FailedToRegisterEventHandler,
//             $"Failed to register Event Handler: {EventProcessor.Value}. An error occurred. {exception.Message}");
//     }
//
//     async Task<bool> RegisterEventProcessorStreamProcessor()
//     {
//         _logger.RegisteringStreamProcessorForEventProcessor(EventProcessor, TargetStream);
//         return await RegisterStreamProcessor(
//             "EventHandler-EventProcessor",
//             FilteredStreamDefinition,
//             _eventProcessorForTenant,
//             HandleFailedToRegisterEventProcessor,
//             (streamProcessor) =>
//             {
//                 EventProcessorStreamProcessor = streamProcessor;
//                 streamProcessor.OnProcessedEvent += (tenant, @event, time) =>
//                 {
//                     _metrics.IncrementEventsProcessedTotal(Info, tenant, @event, time);
//                 };
//                 streamProcessor.OnFailedToProcessedEvent += (tenant, @event, time) =>
//                 {
//                     _metrics.IncrementEventsProcessedTotal(Info, tenant, @event, time);
//                     _metrics.IncrementEventProcessingFailuresTotal(Info, tenant, @event);
//                 };
//             }).ConfigureAwait(false);
//     }
//
//     Failure HandleFailedToRegisterEventProcessor(Exception exception)
//     {
//         _logger.ErrorWhileRegisteringStreamProcessorForEventProcessor(exception, EventProcessor);
//
//         if (exception is not StreamProcessorAlreadyRegistered)
//         {
//             return new Failure(
//                 EventHandlersFailures.FailedToRegisterEventHandler,
//                 $"Failed to register Event Handler: {EventProcessor.Value}. An error occurred. {exception.Message}");
//         }
//
//         _logger.EventHandlerAlreadyRegisteredOnSourceStream(EventProcessor);
//         return new Failure(
//             EventHandlersFailures.FailedToRegisterEventHandler,
//             $"Failed to register Event Handler: {EventProcessor.Value}. Event Processor already registered on Source Stream: '{EventProcessor.Value}'");
//     }
//
//     async Task<bool> RegisterStreamProcessor(
//         EventProcessorKind kind,
//         IStreamDefinition streamDefinition,
//         Func<TenantId, IEventProcessor> getProcessor,
//         Func<Exception, Failure> onException,
//         Action<StreamProcessor> onStreamProcessor)
//     {
//         var streamProcessor = _streamProcessors.TryCreateAndRegister(
//             Scope,
//             EventProcessor,
//             kind,
//             streamDefinition,
//             getProcessor,
//             _executionContext,
//             _cancellationTokenSource.Token);
//
//         onStreamProcessor(streamProcessor);
//
//         if (!streamProcessor.Success)
//         {
//             await Fail(onException(streamProcessor.Exception)).ConfigureAwait(false);
//         }
//
//         return streamProcessor.Success;
//     }
//
//     async Task Start()
//     {
//         _logger.StartingEventHandler(FilterDefinition.TargetStream);
//         try
//         {
//             var runningDispatcher = _acceptRegistration(_cancellationTokenSource.Token);
//             await FilterStreamProcessor.Initialize().ConfigureAwait(false);
//             await EventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
//             await ValidateFilter().ConfigureAwait(false);
//
//             var tasks = new TaskGroup(FilterStreamProcessor.Start(), EventProcessorStreamProcessor.Start(), runningDispatcher);
//             
//             tasks.OnFirstTaskFailure += (_, ex) => _logger.ErrorWhileRunningEventHandler(ex, EventProcessor, Scope);
//             tasks.OnAllTasksCompleted += () => _logger.EventHandlerDisconnected(EventProcessor, Scope);
//
//             await tasks.WaitForAllCancellingOnFirst(_cancellationTokenSource).ConfigureAwait(false);
//         }
//         catch (Exception ex)
//         {
//             if (!_cancellationTokenSource.Token.IsCancellationRequested)
//             {
//                 _logger.ErrorWhileStartingEventHandler(ex, EventProcessor, Scope);
//             }
//             ExceptionDispatchInfo.Capture(ex).Throw();
//         }
//     }
//
//     async Task ValidateFilter()
//     {
//         _logger.ValidatingFilter(FilterDefinition.TargetStream);
//         var filterValidationResults = await _filterValidator.Validate(_filterProcessorForTenant, _cancellationTokenSource.Token).ConfigureAwait(false);
//
//         if (filterValidationResults.Any(_ => !_.Value.Success))
//         {
//             var firstFailedValidation = filterValidationResults.First(_ => !_.Value.Success).Value;
//             _logger.FilterValidationFailed(FilterDefinition.TargetStream, firstFailedValidation.FailureReason);
//             throw new FilterValidationFailed(FilterDefinition.TargetStream, firstFailedValidation.FailureReason);
//         }
//
//         var filteredStreamDefinition = new StreamDefinition(FilterDefinition);
//         _logger.PersistingStreamDefinition(filteredStreamDefinition.StreamId);
//         await _streamDefinitions.Persist(Scope, filteredStreamDefinition, _cancellationTokenSource.Token).ConfigureAwait(false);
//     }
//
//     Task Fail(FailureId failureId, string message)
//         => Fail(new Failure(failureId, message));
//
//     Task Fail(Failure failure)
//     {
//         OnRegistrationFailed?.Invoke();
//         return _rejectRegistration(failure, _cancellationTokenSource.Token);
//     }
// }
