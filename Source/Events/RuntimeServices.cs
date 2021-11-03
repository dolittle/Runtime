// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly EventTypesService _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="EventTypesService"/>.</param>
        public RuntimeServices(EventTypesService eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Events";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new[]
            {
                new Service(_eventTypes, Contracts.EventTypes.BindService(_eventTypes), Contracts.EventTypes.Descriptor)
            };
    }
}
