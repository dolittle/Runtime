// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams;

public interface IStreamEventSubscriber
{
    public ChannelReader<StreamEvent> SubscribePublic(ProcessingPosition position, CancellationToken cancellationToken);

    /// <summary>
    /// Subscribe to a stream of events for a specific scope and a set of event types.
    /// </summary>
    /// <param name="scopeId">The source scope</param>
    /// <param name="artifactIds">The set of subscribed events</param>
    /// <param name="position">The <see cref="ProcessingPosition"/> to continue from (inclusive)</param>
    /// <param name="partitioned">Whether the events should be tagged with "partitioned"</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<StreamEvent> Subscribe(ScopeId scopeId, IEnumerable<ArtifactId> artifactIds, ProcessingPosition position, bool partitioned,
        CancellationToken cancellationToken);
}
