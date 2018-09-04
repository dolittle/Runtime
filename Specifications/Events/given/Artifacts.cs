namespace Dolittle.Runtime.Events.Specs.given
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dolittle.Applications;
    using Dolittle.Artifacts;
    using Dolittle.Collections;
    using Dolittle.Events;
    using Dolittle.Execution;
    using Dolittle.Runtime.Events.Store;

    public static class Artifacts
    {
        public static Artifact artifact_for_simple_event = new Artifact(Guid.NewGuid(),1);
        public static Artifact artifact_for_another_event = new Artifact(Guid.NewGuid(),1);

        public static Artifact artifact_for_event_source = new Artifact(Guid.NewGuid(),1);
    }
}