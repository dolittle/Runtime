/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Artifacts;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the key for an event source (the <see cref="EventSourceId">id </see> and <see cref="ArtifactId">artifact</see>)
    /// </summary>
    public class EventSourceKey : Value<EventSourceKey>
    {
        /// <summary>
        /// Instantiates a new instance of an <see cref="EventSourceKey"/> with the Event Source Id and ArtifactId
        /// </summary>
        /// <param name="id">The Event Source Id</param>
        ///  <param name="artifact">The Event Source Artifact</param>
        public EventSourceKey(EventSourceId id, ArtifactId artifact) 
        {
            Id = id;
            Artifact = artifact;
        }

        /// <summary>
        /// Gets the <see cref="EventSourceId" />
        /// </summary>
        public EventSourceId Id { get; }
        /// <summary>
        /// Gets the <see cref="Artifact" />
        /// </summary>
        public ArtifactId Artifact { get; }
    }
}
