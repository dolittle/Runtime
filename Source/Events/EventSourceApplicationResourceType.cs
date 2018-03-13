/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events
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
