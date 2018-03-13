/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Events.Publishing
{
    /// <summary>
    /// Defines something that is capable of sending <see cref="CommittedEventStream"/> 
    /// </summary>
    public interface ICanSendCommittedEventStream
    {
        /// <summary>
        /// Sends a <see cref="CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEventStream"/> to send</param>
        void Send(CommittedEventStream committedEventStream);
    }
}
