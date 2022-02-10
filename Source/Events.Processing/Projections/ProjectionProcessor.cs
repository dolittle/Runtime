// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing.Projections;

public class ProjectionProcessor : IDisposable
{
    public ProjectionProcessor(IProjection projection, StreamProcessor streamProcessor, Action unregister)
    {
    }

    public Task Start(CancellationToken cancellationToken)
    {
    }

    public void Dispose()
    {
    }
}
