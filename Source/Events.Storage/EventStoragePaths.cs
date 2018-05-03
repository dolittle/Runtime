/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using HandlebarsDotNet;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStoragePaths"/>
    /// </summary>
    public class EventStoragePaths : IEventStoragePaths
    {
        static string[] _templates = {
            "{{ApplicationName}}/{{Tenant}}//{{EventSourceId}}",
            "{{ApplicationName}}/{{Tenant}}//{{DateTime}}/{{EventSourceId}}"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templates"></param>
        public EventStoragePaths(IEventStoragePathTemplates templates)
        {

        }



        /// <inheritdoc/>
        public IEnumerable<EventStoragePath> GetForContext(EventSourceId eventSourceId)
        {
            throw new NotImplementedException();
        }
    }
}