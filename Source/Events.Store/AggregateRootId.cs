// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the identifier for an Aggregate Root.
    /// </summary>
    /// <param name="Id">The id of the Aggregate Root.</param>
    /// <param name="Generation">The generation of the Aggregate Root.</param>
    public record AggregateRootId(ArtifactId Id, ArtifactGeneration Generation) : Artifact(Id, Generation)
    {
    }
}
