// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the result of committing events.
    /// </summary>
    /// <typeparam name="TEvents">Type of the committed events sequence.</typeparam>
    public class CommitEventsResult<TEvents>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitEventsResult{TEvents}"/> class.
        /// </summary>
        /// <param name="events">The committed events.</param>
        public CommitEventsResult(TEvents events)
        {
            Events = events;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitEventsResult{TEvents}"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="EventStoreFailure" />.</param>
        public CommitEventsResult(EventStoreFailure failure)
        {
            Failure = failure;
        }

        /// <summary>
        /// Gets a value indicating whether the events were committed successfully.
        /// </summary>
        public bool Success => Failure == null;

        /// <summary>
        /// Gets the committed events sequence.
        /// </summary>
        public TEvents Events { get; }

        /// <summary>
        /// Gets the <see cref="EventStoreFailure" />.
        /// </summary>
        public EventStoreFailure Failure { get; }
    }
}