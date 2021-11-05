// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> that exposes Events management services.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly EventTypesService _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="eventTypes">The <see cref="EventTypesService"/>.</param>
        public ManagementServices(EventTypesService eventTypes)
        {
            _eventTypes = eventTypes;
        }

        /// <inheritdoc />
        public ServiceAspect Aspect => "Events.Management";

        /// <inheritdoc />
        public IEnumerable<Service> BindServices() =>
            new[]
            {
                new Service(_eventTypes, Contracts.EventTypes.BindService(_eventTypes), Contracts.EventTypes.Descriptor),
            };
    }
}
