// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Coordination
{
    /// <summary>
    /// Defines a coordinator for dealing with <see cref="UncommittedEvents"/>.
    /// </summary>
    public interface IUncommittedEventStreamCoordinator
    {
        /// <summary>
        /// Commit a <see cref="UncommittedEvents"/>.
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="CorrelationId"/> related to the request
        /// the <see cref="UncommittedAggregateEvents"/> was generated in.
        /// </param>
        /// <param name="events"><see cref="UncommittedAggregateEvents"/> to commit.</param>
        void Commit(CorrelationId correlationId, UncommittedAggregateEvents events);
    }
}
