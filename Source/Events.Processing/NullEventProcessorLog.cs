/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a null implementation of <see cref="IEventProcessorLog"/>
    /// </summary>

    public class NullEventProcessorLog : IEventProcessorLog
    {
        /// <inheritdoc/>
        public void Failed(IEventProcessor processor, IEvent @event, IEventEnvelope envelope, IEnumerable<EventProcessingMessage> messages)
        {
            
        }

        /// <inheritdoc/>
        public void Info(IEventProcessor processor, IEvent @event, IEventEnvelope envelope, IEnumerable<EventProcessingMessage> messages)
        {
            
        }
    }
}
