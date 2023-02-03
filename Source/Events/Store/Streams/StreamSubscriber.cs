// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;

namespace Dolittle.Runtime.Events.Store.Streams;

[Singleton, PerTenant]
public class StreamSubscriber : IStreamSubscriber
{
    readonly IEventLogStream _eventLogStream;

    public StreamSubscriber(IEventLogStream eventLogStream) => _eventLogStream = eventLogStream;

    public async IAsyncEnumerable<StreamEvent> SubscribePublic(ProcessingPosition position, CancellationToken cancellationToken)
    {
        // var current = position;
        // var channel = _eventLogStream.SubscribePublic(ScopeId.Default, position.EventLogPosition, cancellationToken);
        // var eventLogBatch = await channel.ReadAsync(cancellationToken);

        throw new NotImplementedException("TODO");
        yield break;
    }
}
