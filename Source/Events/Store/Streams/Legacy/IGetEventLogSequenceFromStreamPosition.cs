// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

public interface IGetEventLogSequenceFromStreamPosition
{
    Task<Try<EventLogSequenceNumber>> TryGetEventLogPositionForStreamProcessor(StreamProcessorId id, StreamPosition streamPosition,
        CancellationToken cancellationToken);
}
