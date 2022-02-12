// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Reflection;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a Projection processor.
/// </summary>
public class ProjectionProcessor : IDisposable
{
    readonly IProjection _projection;
    readonly StreamProcessor _streamProcessor;
    readonly Action _unregister;
    readonly ILogger _logger;
    readonly CancellationToken _cancellationToken;

    public ProjectionProcessor(IProjection projection, StreamProcessor streamProcessor, Action unregister, ILogger logger, CancellationToken cancellationToken)
    {
        _projection = projection;
        _streamProcessor = streamProcessor;
        _unregister = unregister;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the <see cref="ProjectionDefinition"/> for this projection processor.
    /// </summary>
    public ProjectionDefinition Definition => _projection.Definition;

    /// <summary>
    /// Gets the <see cref="ProjectionInfo"/> for this projection processor.
    /// </summary>
    public ProjectionInfo Info => new(
        _projection.Definition,
        _projection.HasAlias,
        _projection.Alias);

    /// <summary>
    /// Starts the projection processor.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous processing.</returns>
    public async Task Start()
    {
        Task startedProcessor;
        try
        {
            Log.StartingProjection(_logger, Definition.Scope, Definition.Projection);
            
            await _streamProcessor.Initialize().ConfigureAwait(false);
            startedProcessor = _streamProcessor.Start();
        }
        catch (Exception exception)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                return;
            }
            
            Log.ErrorWhileStartingProjection(_logger, Definition.Scope, Definition.Projection, exception);
            throw;
        }

        await startedProcessor.ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all current <see cref="IStreamProcessorState"/> states for this <see cref="ProjectionProcessor"/>. 
    /// </summary>
    /// <returns>The <see cref="IStreamProcessorState"/> per <see cref="TenantId"/>.</returns>
    public Try<IDictionary<TenantId, IStreamProcessorState>> GetCurrentStates()
        => _streamProcessor.GetCurrentStates();

    public async Task<Try> ReplayEventsForTenant(TenantId tenant, Func<TenantId, CancellationToken, Task<Try>> dropStates)
    {
        return await _streamProcessor.PerformActionAndSetToPosition(tenant, StreamPosition.Start, dropStates);
    }

    public async Task<IDictionary<TenantId, Try>> ReplayEventsForAllTenants(Func<TenantId, CancellationToken, Task<Try>> dropStates)
    {
        var results = await _streamProcessor.PerformActionAndSetToInitialPositionForAllTenants(dropStates).ConfigureAwait(false);
        return results.ToDictionary(_ => _.Key, _ => (Try)_.Value);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _streamProcessor.Dispose();
        _unregister();
        GC.SuppressFinalize(this);
    }
}
