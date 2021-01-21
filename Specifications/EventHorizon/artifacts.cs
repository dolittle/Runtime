// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.EventHorizon
{
    public static class artifacts
    {
        public static Artifact create() => new Artifact(ArtifactId.New(), 1);
    }
}
