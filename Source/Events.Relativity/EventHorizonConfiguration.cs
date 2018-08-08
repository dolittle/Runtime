/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the configuration of known <see cref="IEventHorizon"/> to tunnel to
    /// </summary>
    public class EventHorizonConfiguration
    {
        /// <summary>
        /// Gets or sets the Url where the <see cref="IEventHorizon"/> lives
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the events it is interested in, in the form of <see cref="Artifact"/>
        /// </summary>
        public IEnumerable<Artifact> Events { get; set; } = new Artifact[0];
    }
}
