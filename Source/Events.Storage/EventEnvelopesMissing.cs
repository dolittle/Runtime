/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Exception that gets thrown if <see cref="IEventEnvelopes"/> is not provided e.g. for <see cref="EventSource"/>
    /// </summary>
    public class EventEnvelopesMissing : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventEnvelopesMissing"/>
        /// </summary>
        public EventEnvelopesMissing() : base("Event Enevelopes is missing") { }
    }
}
