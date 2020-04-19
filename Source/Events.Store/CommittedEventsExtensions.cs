// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using System.Linq;
using grpc = contracts::Dolittle.Runtime.Events.Contracts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extension methods for <see cref="CommittedEvents" />.
    /// </summary>
    public static class CommittedEventsExtensions
    {
        /// <summary>
        /// Converts the <see cref="CommittedEvents" /> to <see cref="IEnumerable{T}" /> of <see cref="grpc.CommittedEvent" />.
        /// </summary>
        /// <param name="committedEvents">The committed events.</param>
        /// <returns>The converted <see cref="IEnumerable{T}" /> of <see cref="grpc.CommittedEvent" />.</returns>
        public static IEnumerable<grpc.CommittedEvent> ToProtobuf(this CommittedEvents committedEvents) =>
            committedEvents.AsEnumerable().Select(_ => _.ToProtobuf());
    }
}