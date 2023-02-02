// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Dolittle.Runtime.Domain.Tenancy;
// using Dolittle.Runtime.Events.Processing.Streams;
// using Dolittle.Runtime.Events.Store.Streams.Filters;
// using Dolittle.Runtime.Rudimentary;
// using Dolittle.Runtime.Tenancy;
// using Microsoft.Extensions.Logging;
//
// namespace Dolittle.Runtime.Events.Processing.EventHandlers;
//
// /// <summary>
// /// Represents a fast stream processor meant to be used for the filter of an event handler.
// /// TODO: This should be replaced at some point 
// /// </summary>
// public class FilterStreamProcessor : IDisposable
// {
//     readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
//     readonly Action _unregister;
//     readonly StreamProcessorId _identifier;
//     readonly IPerformActionsForAllTenants _forAllTenants;
//     readonly Func<TenantId, ICreateScopedFilterStreamProcessors> _getCreateScopedStreamProcessors;
//     readonly ILogger _logger;
//     readonly CancellationTokenSource _stopAllScopedStreamProcessorsTokenSource;
//     
//     readonly Dictionary<TenantId, ScopedFilterStreamProcessor> _streamProcessors = new();
//     bool _initialized;
//     bool _started;
//     bool _disposed;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="FilterStreamProcessor"/> class.
//     /// </summary>
//     /// <param name="streamProcessorId">The stream processor id.</param>
//     /// <param name="filterDefinition">The filter definition.</param>
//     /// <param name="unregister">The action to perform unregistering</param>
//     /// <param name="forAllTenants">The performer to use to create scoped stream processors for all tenants.</param>
//     /// <param name="getCreateScopedStreamProcessors">The factory to us to get the scoped stream processor creator per tenant.</param>
//     /// <param name="logger">The logger to use for logging.</param>
//     /// <param name="cancellationToken">The cancellation token that is cancelled when the stream processor should stop processing.</param>
//     public FilterStreamProcessor(
//         StreamProcessorId streamProcessorId,
//         TypeFilterWithEventSourcePartitionDefinition filterDefinition,
//         Action unregister,
//         IPerformActionsForAllTenants forAllTenants,
//         Func<TenantId, ICreateScopedFilterStreamProcessors> getCreateScopedStreamProcessors,
//         ILogger logger,
//         CancellationToken cancellationToken)
//     {
//         _identifier = streamProcessorId;
//         _filterDefinition = filterDefinition;
//         _unregister = unregister;
//         _forAllTenants = forAllTenants;
//         _getCreateScopedStreamProcessors = getCreateScopedStreamProcessors;
//         _logger = logger;
//         _stopAllScopedStreamProcessorsTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
//     }
//     /// <summary>
//     /// Initializes the stream processor.
//     /// </summary>
//     /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
//     public async Task Initialize()
//     {
//         _logger.InitializingFilterStreamProcessor(_identifier);
//
//         _stopAllScopedStreamProcessorsTokenSource.Token.ThrowIfCancellationRequested();
//         if (_initialized)
//         {
//             throw new StreamProcessorAlreadyInitialized(_identifier);
//         }
//         _initialized = true;
//
//         await _forAllTenants.PerformAsync(async (tenant, _) =>
//         {
//             var scopedStreamProcessor = await _getCreateScopedStreamProcessors(tenant).Create(
//                 _filterDefinition,
//                 _identifier,
//                 _stopAllScopedStreamProcessorsTokenSource.Token).ConfigureAwait(false);
//             
//             _streamProcessors.Add(tenant, scopedStreamProcessor);
//         }).ConfigureAwait(false);
//     }
//
//     /// <summary>
//     /// Starts the stream processing for all tenants.
//     /// </summary>
//     /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
//     public async Task Start()
//     {
//         _logger.StartingFilterStreamProcessor(_identifier);
//
//         if (!_initialized)
//         {
//             throw new StreamProcessorNotInitialized(_identifier);
//         }
//
//         if (_started)
//         {
//             throw new StreamProcessorAlreadyProcessingStream(_identifier);
//         }
//
//         _started = true;
//         try
//         {
//             var tasks = new TaskGroup(StartScopedStreamProcessors(_stopAllScopedStreamProcessorsTokenSource.Token));
//         
//             tasks.OnFirstTaskFailure += (_, ex) => _logger.ScopedFilterStreamProcessorFailed(ex, _identifier);
//             await tasks.WaitForAllCancellingOnFirst(_stopAllScopedStreamProcessorsTokenSource).ConfigureAwait(false);
//         }
//         finally
//         {
//             _unregister();
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
//     /// Dispose the object.
//     /// </summary>
//     /// <param name="disposing">Whether to dispose managed state.</param>
//     protected virtual void Dispose(bool disposing)
//     {
//         if (_disposed)
//         {
//             return;
//         }
//
//         if (disposing)
//         {
//             _stopAllScopedStreamProcessorsTokenSource.Cancel();
//             _stopAllScopedStreamProcessorsTokenSource.Dispose();
//             
//             if (!_started)
//             {
//                 _unregister();
//             }
//         }
//
//         _disposed = true;
//     }
//
//     IEnumerable<Task> StartScopedStreamProcessors(CancellationToken cancellationToken) => _streamProcessors.Select(
//         _ => Task.Run(async () =>
//         {
//             var (_, streamProcessor) = _;
//             await streamProcessor.Start(cancellationToken).ConfigureAwait(false);
//         }, cancellationToken)).ToList();
//
// }
