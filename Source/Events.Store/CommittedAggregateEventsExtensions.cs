// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using System.Linq;
using Dolittle.Protobuf;
using grpc = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extension methods for <see cref="CommittedAggregateEvents" />.
    /// </summary>
    public static class CommittedAggregateEventsExtensions
    {
        /// <summary>
        /// Converts the list of <see cref="CommittedAggregateEvent" /> to a list of <see cref="grpc.CommittedAggregateEvent" />s.
        /// </summary>
        /// <param name="committedEvents">The committed events.</param>
        /// <returns>The converted list of <see cref="grpc.CommittedAggregateEvent" />.</returns>
        public static IEnumerable<grpc.CommittedAggregateEvent> ToProtobuf(this IEnumerable<CommittedAggregateEvent> committedEvents) => committedEvents.Select(_ => _.ToProtobuf());

        /// <summary>
        /// Converts the <see cref="CommittedAggregateEvents" /> to <see cref="grpc.CommittedAggregateEvents" />s.
        /// </summary>
        /// <param name="committedAggregateEvents">The committed events.</param>
        /// <returns>The converted <see cref="grpc.CommittedAggregateEvents" />.</returns>
        public static grpc.CommittedAggregateEvents ToProtobuf(this CommittedAggregateEvents committedAggregateEvents)
        {
            var protobuf = new grpc.CommittedAggregateEvents
            {
                AggregateRoot = committedAggregateEvents.AggregateRoot.Value.ToProtobuf(),
                EventSource = committedAggregateEvents.EventSource.ToProtobuf()
            };
            protobuf.Events.AddRange(committedAggregateEvents.AsEnumerable().ToProtobuf());
            return protobuf;
        }
    }
}