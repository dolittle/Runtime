// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class random
    {
        public static AggregateRootVersion aggregate_root_version => new AggregateRootVersion { Value = (ulong)new Random().Next() };

        public static EventLogSequenceNumber event_log_sequence_number => new EventLogSequenceNumber { Value = (ulong)new Random().Next() };

        public static StreamPosition stream_position => new StreamPosition((ulong)new Random().Next());

        public static int natural_number => new Random().Next();
    }
}