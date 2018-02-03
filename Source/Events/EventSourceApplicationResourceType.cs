/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using doLittle.Artifacts;

namespace doLittle.Runtime.Events
{
    /// <summary>
    /// Represents a <see cref="IArtifactType">artifact type</see> for <see cref="IEventSource">events</see>
    /// </summary>
    public class EventSourceApplicationResourceType : IArtifactType
    {
        /// <inheritdoc/>
        public string Identifier => "EventSource";
    }
}
