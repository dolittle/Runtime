/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Publishing.InProcess
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanSendCommittedEventStream"/> for in-process purpose
    /// </summary>
    public class CommittedEventStreamSender : ICanSendCommittedEventStream
    {
        readonly ICommittedEventStreamBridge _bridge;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CommittedEventStreamSender"/>
        /// </summary>
        /// <param name="bridge"><see cref="ICommittedEventStreamBridge"/> to use for bridging</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public CommittedEventStreamSender(ICommittedEventStreamBridge bridge, ILogger logger)
        {
            _bridge = bridge;
            _logger = logger;
        }


        /// <inheritdoc/>
        public void Send(CommittedEventStream committedEventStream)
        {
            _logger.Trace("Sending committed event stream");
            _bridge.Send(committedEventStream);
        }
    }
}
