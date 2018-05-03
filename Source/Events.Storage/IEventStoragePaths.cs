/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system for generating <see cref="EventStoragePath"/> based on variables
    /// </summary>
    public interface IEventStoragePaths
    {
        /// <summary>
        /// Get the 
        /// </summary>
        /// <param name="eventSourceId"></param>
        /// <returns></returns>
        IEnumerable<EventStoragePath>   GetForContext(EventSourceId eventSourceId);
    }
}