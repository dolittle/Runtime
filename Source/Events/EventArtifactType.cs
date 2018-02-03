/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using doLittle.Artifacts;

namespace doLittle.Events
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
