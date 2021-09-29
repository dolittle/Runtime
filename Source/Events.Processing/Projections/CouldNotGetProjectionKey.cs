// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Exception that gets throw when getting the projection key from an event fails.
    /// </summary>
    public class CouldNotGetProjectionKey : Exception
    {
        public CouldNotGetProjectionKey(CommittedEvent @event)
            : base($"Could not get projection key from event on sequence number {@event.EventLogSequenceNumber} with content '{@event.Content}'")
        {
        }
    }
}