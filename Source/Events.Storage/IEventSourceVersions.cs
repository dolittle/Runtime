/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Applications;
using doLittle.Events;

namespace doLittle.Runtime.Events.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventSourceVersions
    {
        /// <summary>
        /// Gets the version of an <see cref="IEventSource"/>
        /// </summary>
        /// <param name="eventSource"><see cref="IApplicationArtifactIdentifier">Identifer</see> representing the <see cref="IEventSource"/></param>
        /// <param name="eventSourceId"><see cref="EventSourceId"/> of the <see cref="IEventSource"/> </param>
        /// <returns><see cref="EventSourceVersion"/> of the <see cref="IEventSource"/></returns>
        /// <remarks>
        /// If there is no version registration or <see cref="IEvent"/> for the <see cref="IEventSource"/>
        /// It will return <see cref="EventSourceVersion.Zero"/>
        /// </remarks>
        EventSourceVersion GetFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId);

        /// <summary>
        /// Sets the version of an <see cref="IEventSource"/>
        /// </summary>
        /// <param name="eventSource"><see cref="IApplicationArtifactIdentifier">Identifer</see> representing the <see cref="IEventSource"/></param>
        /// <param name="eventSourceId"><see cref="EventSourceId"/> of the <see cref="IEventSource"/> </param>
        /// <param name="version"><see cref="EventSourceVersion"/> of the <see cref="IEventSource"/></param>
        void SetFor(IApplicationArtifactIdentifier eventSource, EventSourceId eventSourceId, EventSourceVersion version);
    }
}