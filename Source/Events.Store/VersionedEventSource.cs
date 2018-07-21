using System;
using Dolittle.Concepts;
using Dolittle.Applications;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A unique identifier of a particular EventSource (instance, type and version)
    /// </summary>
    public class VersionedEventSource : Value<VersionedEventSource>
    {
        /// <summary>
        /// Instantiates a new instance of a <see cref="VersionedEventSource" /> set to the initial version
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" /></param>
        /// <param name="artifact">The <see cref="IApplicationArtifactIdentifier" /> that uniquely identifies the type of this event source</param>
        /// <returns></returns>
        public VersionedEventSource(EventSourceId eventSource, IApplicationArtifactIdentifier artifact): this(EventSourceVersion.Initial(), eventSource, artifact) { }

        /// <summary>
        /// Instantiates a new instance of a <see cref="VersionedEventSource" /> set to the supplied version
        /// </summary>
        /// <param name="version">The <see cref="EventSourceVersion" /> of this instance</param>
        /// <param name="eventSource">The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" /></param>
        /// <param name="artifact">The <see cref="IApplicationArtifactIdentifier" /> that uniquely identifies the type of this event source</param>
        public VersionedEventSource(EventSourceVersion version, EventSourceId eventSource, IApplicationArtifactIdentifier artifact)
        {
            Version = version;
            EventSource = eventSource;
            Artifact = artifact;
        }
        /// <summary>
        /// The <see cref="EventSourceVersion" /> of this instance
        /// </summary>
        /// <value></value>
        public EventSourceVersion Version { get; }
        /// <summary>
        /// The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" />
        /// </summary>
        /// <value></value>
        public EventSourceId EventSource { get; }
        /// <summary>
        /// The <see cref="ApplicationArtifactIdentifier" /> that uniquely identifies the type of this event source
        /// </summary>
        /// <value></value>
        public IApplicationArtifactIdentifier Artifact { get; }
    }
}