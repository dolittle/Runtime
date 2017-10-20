/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using doLittle.Runtime.Applications;
using doLittle.Execution;
using doLittle.Logging;
using doLittle.Types;
using doLittle.Events;

namespace doLittle.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="ApplicationResourceResolverFor{T}"/> that
    /// knows how to resolve <see cref="IEvent">events</see>
    /// </summary>
    public class EventApplicationResourceResolver : ApplicationResourceResolverFor<EventApplicationResourceType>
    {
        IApplication _application;
        IEnumerable<Type> _eventTypes;
        ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventApplicationResourceResolver"/>
        /// </summary>
        /// <param name="application">Current <see cref="IApplication"/></param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for finding correct <see cref="IEvent">event type</see></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public EventApplicationResourceResolver(IApplication application, ITypeFinder typeFinder, ILogger logger)
        {
            _application = application;
            _eventTypes = typeFinder.FindMultiple<IEvent>();
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Type Resolve(IApplicationResourceIdentifier identifier)
        {
            _logger.Trace($"Trying to resolve for {identifier.Resource.Name}");

            var formats = _application.Structure.GetStructureFormatsForArea(ApplicationAreas.Events);
            var types = _eventTypes.Where(t => t.Name == identifier.Resource.Name);

            foreach (var type in types)
            {
                foreach (var format in formats)
                {
                    if (format.Match(type.Namespace).HasMatches) return type;
                }
            }

            throw new UnknownApplicationResourceType(identifier.Resource.Type.Identifier);
        }
    }
}