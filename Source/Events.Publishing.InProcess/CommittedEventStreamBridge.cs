/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Publishing.InProcess
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommittedEventStreamBridge"/>
    /// </summary>
    [Singleton]
    public class CommittedEventStreamBridge : ICommittedEventStreamBridge
    {
        static event CommittedEventStreamReceived _received = (s) => { };

        /// <inheritdoc/>
        public event CommittedEventStreamReceived Received
        {
            add { _received += value; }
            remove { _received -= value; }
        }

        /// <inheritdoc/>
        public void Send(CommittedEventStream committedEventStream)
        {
            _received(committedEventStream);
        }
    }
}
