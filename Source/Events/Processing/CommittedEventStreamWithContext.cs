// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a combination of a <see cref="CommittedEventStream" /> and the related <see cref="ExecutionContext" />.
    /// </summary>
    public class CommittedEventStreamWithContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventStreamWithContext"/> class.
        /// </summary>
        /// <param name="stream">The committed event stream.</param>
        /// <param name="context">The execution context.</param>
        public CommittedEventStreamWithContext(CommittedEventStream stream, ExecutionContext context)
        {
            EventStream = stream;
            Context = context;
        }

        /// <summary>
        /// Gets the <see cref="CommittedEventStream" />.
        /// </summary>
        public CommittedEventStream EventStream { get; }

        /// <summary>
        /// Gets the <see cref="ExecutionContext" />.
        /// </summary>
        public ExecutionContext Context { get; }
    }
}