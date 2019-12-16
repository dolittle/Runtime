// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Specs.given
{
    public static class Artifacts
    {
        public static Artifact artifact_for_simple_event = new Artifact(Guid.NewGuid(), 1);
        public static Artifact artifact_for_another_event = new Artifact(Guid.NewGuid(), 1);
        public static Artifact artifact_for_event_source = new Artifact(Guid.NewGuid(), 1);
    }
}