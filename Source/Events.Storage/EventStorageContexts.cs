/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStorageContexts"/>
    /// </summary>
    public class EventStorageContexts : IEventStorageContexts
    {

        /// <inheritdoc/>
        public EventStorageContext GetCurrentFor(EventSourceId eventSourceId)
        {
            throw new System.NotImplementedException();
        }
    }
}