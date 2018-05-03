/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Dolittle.Collections;
using Dolittle.Types;
using HandlebarsDotNet;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventStoragePathTemplates"/>
    /// </summary>
    public class EventStoragePathTemplates : IEventStoragePathTemplates
    {
        readonly List<EventStoragePathTemplate> _all = new List<EventStoragePathTemplate>();

        /// <summary>
        /// Initializes a new instance of <see cref="EventStoragePathTemplates"/>
        /// </summary>
        /// <param name="providers"><see cref="ICanProvideEventStoragePathTemplates">Providers</see> that can provide string versions of the template</param>
        public EventStoragePathTemplates(IInstancesOf<ICanProvideEventStoragePathTemplates> providers)
        {
            providers.ForEach(_ => _.Provide().ForEach(Add));
        }
        
        /// <inheritdoc/>
        public IEnumerable<EventStoragePathTemplate> All => _all;

        /// <inheritdoc/>
        public void Add(string template)
        {
            var handlebarsFunction = Handlebars.Compile(template);
            Func<EventStorageContext, string> function = (EventStorageContext context) => handlebarsFunction(context);
            _all.Add(new EventStoragePathTemplate(template, function));
        }
    }
}