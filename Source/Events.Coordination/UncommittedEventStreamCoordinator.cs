/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;
using Dolittle.Runtime.Transactions;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Coordination
{
    /// <summary>
    /// Represents a <see cref="IUncommittedEventStreamCoordinator"/>
    /// </summary>
    [Singleton]
    public class UncommittedEventStreamCoordinator : IUncommittedEventStreamCoordinator
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes an instance of a <see cref="UncommittedEventStreamCoordinator"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for doing logging</param>
        public UncommittedEventStreamCoordinator(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Commit(TransactionCorrelationId correlationId, UncommittedEventStream uncommittedEventStream)
        {
            // Steps:
            // - Save to the event store to get a committed event stream
            // - in parallel
            //   - Send it off to the event horizon
            //   - Process in current bounded context (IEventProcessors)

            _logger.Trace("Send the committed event stream");
        }
    }
}
