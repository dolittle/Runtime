// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using System.Linq;
using grpc = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extension methods for <see cref="CommittedEvents" />.
    /// </summary>
    public static class CommittedEventsExtensions
    {
        /// <summary>
        /// Converts the list of <see cref="CommittedEvent" /> to a list of <see cref="grpc.CommittedEvent" />s.
        /// </summary>
        /// <param name="committedEvents">The committed events.</param>
        /// <returns>The converted list of <see cref="grpc.CommittedEvent" />.</returns>
        public static IEnumerable<grpc.CommittedEvent> ToProtobuf(this IEnumerable<CommittedEvent> committedEvents) => committedEvents.Select(_ => _.ToProtobuf());

        /// <summary>
        /// Converts the <see cref="CommittedEvents" /> to <see cref="grpc.CommittedEvents" />s.
        /// </summary>
        /// <param name="committedEvents">The committed events.</param>
        /// <returns>The converted <see cref="grpc.CommittedEvents" />.</returns>
        public static grpc.CommittedEvents ToProtobuf(this CommittedEvents committedEvents)
        {
            var protobuf = new grpc.CommittedEvents();
            protobuf.Events.AddRange(committedEvents.AsEnumerable().ToProtobuf());
            return protobuf;
        }
    }
}