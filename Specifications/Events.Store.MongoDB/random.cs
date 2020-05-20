// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class random
    {
        public static AggregateRootVersion aggregate_root_version => new AggregateRootVersion { Value = 3961784202 };

        public static EventLogSequenceNumber event_log_sequence_number => new EventLogSequenceNumber { Value = 4286306012 };

        public static StreamPosition stream_position => new StreamPosition(3714684996);

        public static int natural_number => 4875;
    }
}