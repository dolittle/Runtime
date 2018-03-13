/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Events.Publishing
{
    /// <summary>
    /// Represents a delegete for dealing with a received <see cref="CommittedEventStream"/> 
    /// </summary>
    /// <param name="committedEventStream"><see cref="CommittedEventStream"/> that was received</param>
    public delegate void CommittedEventStreamReceived(CommittedEventStream committedEventStream);
}