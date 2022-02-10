// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Projections;

public class ProjectionProcessor : IDisposable
{
    readonly IProjection _projection;
    readonly StreamProcessor _streamProcessor;
    readonly Action _unregister;

    public ProjectionProcessor(IProjection projection, StreamProcessor streamProcessor, Action unregister)
    {
        _projection = projection;
        _streamProcessor = streamProcessor;
        _unregister = unregister;
    }

    public ProjectionDefinition Definition => _projection.Definition;

    public async Task Start()
    {
        await _streamProcessor.Initialize();
        await _streamProcessor.Start();
    }

    /// <summary>
    /// Gets all current <see cref="IStreamProcessorState"/> states for this <see cref="ProjectionProcessor"/>. 
    /// </summary>
    /// <returns>The <see cref="IStreamProcessorState"/> per <see cref="TenantId"/>.</returns>
    public Try<IDictionary<TenantId, IStreamProcessorState>> GetCurrentStates()
        => _streamProcessor.GetCurrentStates();

    public void Dispose()
    {
        //TODO: Implement properly
        _unregister();
    }
}
