using System;
using System.Collections.Concurrent;
using Dolittle.Concepts;
using Dolittle.Applications;

namespace Dolittle.Runtime.Events.Store
{
    //TODO: Swap this out with the real thing from Artifacts

    /// <summary>
    /// Represents a specific combination of Artifact and Generation
    /// </summary>
    public class ArtifactGeneration : Value<ArtifactGeneration>
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="ArtifactGeneration" />
        /// </summary>
        /// <param name="artifact">The artifact</param>
        /// <param name="generation">The generation</param>
        public ArtifactGeneration(IApplicationArtifactIdentifier artifact, Generation generation)
        {
            Artifact = artifact;
            Generation = generation;
        }

        /// <summary>
        /// The artifact that this generation refers to
        /// </summary>
        /// <value></value>
        public IApplicationArtifactIdentifier Artifact { get; }
        /// <summary>
        /// The generation of this artifact
        /// </summary>
        /// <value></value>
        public Generation Generation { get; }
    }
}