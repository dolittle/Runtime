/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;

namespace Dolittle.Runtime.Events.Storage.Files
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStorage"/> for storing to filesystem
    /// </summary>
    public class EventStorage : IEventStorage
    {
        /// <inheritdoc/>
        public Stream GetAppendStreamFor(EventStoragePath path)
        {
            throw new NotImplementedException();
        }
    }
}