/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system for storage of events
    /// </summary>
    public interface IEventStorage
    {
        /// <summary>
        /// Get an append stream for a specific path
        /// </summary>
        /// <param name="path">Path within the storage</param>
        /// <returns></returns>
        Stream GetAppendStreamFor(EventStoragePath path);
    }
}