/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system that can provide <see cref="EventStorageContext"/>
    /// </summary>
    public interface IEventStorageContexts
    {
        /// <summary>
        /// Get current <see cref="EventStorageContext"/>
        /// </summary>
        /// <returns>Current <see cref="EventStorageContext"/></returns>
        EventStorageContext GetCurrentFor(EventSourceId eventSourceId);
    }
}