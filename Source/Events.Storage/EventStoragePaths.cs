/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStoragePaths"/>
    /// </summary>
    public class EventStoragePaths : IEventStoragePaths
    {
        readonly IEventStoragePathTemplates _templates;
        private readonly IEventStorageContexts _eventStorageContexts;

        /*
static string[] _templates = {
"{{ApplicationName}}/{{Tenant}}//{{EventSourceId}}",
"{{ApplicationName}}/{{Tenant}}//{{DateTime}}/{{EventSourceId}}"
};*/

        /// <summary>
        /// Initializes a new instance of <see cref="EventStoragePaths"/>
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="eventStorageContexts"></param>
        public EventStoragePaths(
            IEventStoragePathTemplates templates,
            IEventStorageContexts eventStorageContexts)
        {
            _templates = templates;
            _eventStorageContexts = eventStorageContexts;
        }



        /// <inheritdoc/>
        public IEnumerable<EventStoragePath> GetForContext(EventSourceId eventSourceId)
        {
            var context = _eventStorageContexts.GetCurrentFor(eventSourceId);
            var paths = _templates.All.Select(_ => (EventStoragePath)_.Function(context));
            return paths;
        }
    }
}