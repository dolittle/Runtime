/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the configuration that specifies tunnels to known <see cref="IEventHorizon">event horizons</see>
    /// </summary>
    public class EventHorizonsConfiguration
    {
        /// <summary>
        /// Gets or sets <see cref="IEnumerable{EventHorizonConfiguration}">collection of <see cref="EventHorizonConfiguration"/></see>
        /// </summary>
        public IEnumerable<EventHorizonConfiguration>   EventHorizons { get; set; } = new EventHorizonConfiguration[0];
    }
}
