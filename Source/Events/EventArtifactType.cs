/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Artifacts;

namespace Dolittle.Events
{
    /// <summary>
    /// Represents a <see cref="IArtifactType">application resource type</see> for 
    /// <see cref="IEvent">events</see>
    /// </summary>
    public class EventArtifactType : IArtifactType
    {
        /// <inheritdoc/>
        public string Identifier => "Event";
    }
}
