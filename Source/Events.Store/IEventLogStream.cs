// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

public interface IEventLogStream
{
    /// <summary>
    /// Subscribe to the event log stream from the given offset, filtered by event types.
    /// </summary>
    /// <param name="from">From offset, inclusive</param>
    /// <param name="eventTypes">Included event types, min 1</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<Contracts.CommittedEvent> Subscribe(EventLogSequenceNumber from, IEnumerable<ArtifactId> eventTypes, CancellationToken cancellationToken);
    
    /// <summary>
    /// Subscribe to the complete event log stream at the given offset
    /// </summary>
    /// <param name="from"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<Contracts.CommittedEvent> SubscribeAll(EventLogSequenceNumber from, CancellationToken cancellationToken);
}
