/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Configuration;
using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the configuration that specifies tunnels to known <see cref="IEventHorizon">event horizons</see>
    /// </summary>
    [Name("event-horizons")]
    public class EventHorizonsConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventHorizonsConfiguration"/>
        /// </summary>
        /// <param name="eventHorizons">Collection of <see cref="EventHorizonConfiguration"/></param>
        public EventHorizonsConfiguration(IEnumerable<EventHorizonConfiguration> eventHorizons)
        {
            EventHorizons = eventHorizons;
        }

        /// <summary>
        /// Gets <see cref="IEnumerable{EventHorizonConfiguration}">collection of <see cref="EventHorizonConfiguration"/></see>
        /// </summary>
        public IEnumerable<EventHorizonConfiguration>   EventHorizons { get; }
    }
}
