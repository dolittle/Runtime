// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Store.Actors;

public partial class Bucket
{
    public IStreamProcessorState FromProtobuf()
    {
        return Partitioned ? FromProtobufPartitioned() : FromProtobufNonPartitioned();
    }

    IStreamProcessorState FromProtobufNonPartitioned()
    {
        var lastSuccessfullyProcessed = LastSuccessfullyProcessed.ToDateTimeOffset();
        switch (Failures.Count)
        {
            case 0: return new StreamProcessorState(Position(), "", lastSuccessfullyProcessed, 0, lastSuccessfullyProcessed, false );
            case 1:
                var failure = Failures[0];
                return new StreamProcessorState(Position(), failure.FailureReason, failure.RetryTime.ToDateTimeOffset(), failure.ProcessingAttempts,
                    lastSuccessfullyProcessed, true);
            default:
                // This is probably invalid, as this should not be able to represent more than a single failure
                var fail = Failures[0];
                return new StreamProcessorState(Position(), fail.FailureReason, fail.RetryTime.ToDateTimeOffset(), fail.ProcessingAttempts,
                    lastSuccessfullyProcessed, true);
        }
    }

    StreamPosition Position() => new(CurrentOffset);

    IStreamProcessorState FromProtobufPartitioned()
    {
        var failingPartitions = Failures
            .ToDictionary(kv => new PartitionId(kv.EventSourceId),
                _ => new FailingPartitionState(new StreamPosition(_.Offset), _.RetryTime.ToDateTimeOffset(), _.FailureReason, _.ProcessingAttempts,
                    _.LastFailed.ToDateTimeOffset()));

        return new Processing.Streams.Partitioned.StreamProcessorState(Position(), failingPartitions, LastSuccessfullyProcessed.ToDateTimeOffset());
    }
}
