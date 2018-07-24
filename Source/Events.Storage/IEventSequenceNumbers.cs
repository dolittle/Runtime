/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system for working with <see cref="EventSequenceNumber">event sequence numbers</see>
    /// </summary>
    public interface IEventSequenceNumbers
    {
        /// <summary>
        /// Allocate the next global <see cref="EventSequenceNumber"/>
        /// </summary>
        /// <returns>The next <see cref="EventSequenceNumber"/></returns>
        EventSequenceNumber Next();

        /// <summary>
        /// Allocate the next global <see cref="EventSequenceNumber"/> for a specific <see cref="IEvent">event type</see>
        /// </summary>
        /// <param name="identifier"><see cref="IApplicationArtifactIdentifier">Identifier</see> for the <see cref="IEvent">event type</see></param>
        /// <returns>The next <see cref="EventSequenceNumber"/> for the <see cref="IEvent">event type</see></returns>
        EventSequenceNumber NextForType(IApplicationArtifactIdentifier identifier);
    }
}
