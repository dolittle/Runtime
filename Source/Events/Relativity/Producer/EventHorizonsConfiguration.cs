// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Configuration;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the configuration that specifies tunnels to known <see cref="IEventHorizon">event horizons</see>.
    /// </summary>
    [Name("event-horizons")]
    public class EventHorizonsConfiguration :
        ReadOnlyCollection<EventHorizonConfiguration>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonsConfiguration"/> class.
        /// </summary>
        /// <param name="eventHorizons">Collection of <see cref="EventHorizonConfiguration"/>.</param>
        public EventHorizonsConfiguration(IEnumerable<EventHorizonConfiguration> eventHorizons)
            : base(eventHorizons.ToList())
        {
        }
    }
}
