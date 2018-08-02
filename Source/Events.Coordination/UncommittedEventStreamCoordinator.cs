/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;
using Dolittle.Runtime.Transactions;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Publishing;

namespace Dolittle.Runtime.Events.Coordination
{
    /// <summary>
    /// Represents a <see cref="IUncommittedEventStreamCoordinator"/>
    /// </summary>
    [Singleton]
    public class UncommittedEventStreamCoordinator : IUncommittedEventStreamCoordinator
    {
        readonly ICanSendCommittedEventStream _committedEventStreamSender;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes an instance of a <see cref="UncommittedEventStreamCoordinator"/>
        /// </summary>
        /// <param name="committedEventStreamSender"><see cref="ICanSendCommittedEventStream"/> send the <see cref="CommittedEventStream"/></param>
        /// <param name="logger"><see cref="ILogger"/> for doing logging</param>
        public UncommittedEventStreamCoordinator(
            ICanSendCommittedEventStream committedEventStreamSender,
            ILogger logger)
        {
            _committedEventStreamSender = committedEventStreamSender;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Commit(TransactionCorrelationId correlationId, UncommittedEventStream uncommittedEventStream)
        {
            _logger.Trace("Send the committed event stream");
            //_committedEventStreamSender.Send(committedEventStream);
        }
    }
}
