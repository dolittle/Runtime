// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedEvents.given
{
    public abstract class events
    {
        public static CorrelationId correlation_id = Guid.Parse("4a52a13f-4f74-4ee8-a74d-d1b4b6076d8c");
        public static Microservice microservice_id = Guid.Parse("5d352805-d550-4b12-af6c-58c1e898c84d");
        public static TenantId tenant_id = Guid.Parse("17cda67e-76b9-4c49-bab7-954beb70d357");
        public static Cause cause = new Cause(CauseType.Command, 0);

        public static Artifact event_a_artifact = new Artifact(Guid.Parse("d26cc060-9153-4988-8f07-3cf67f58bf47"), ArtifactGeneration.First);
        public static Artifact event_b_artifact = new Artifact(Guid.Parse("cc657c0a-2c81-4338-85a8-507f05d4fc0e"), ArtifactGeneration.First);

        public static CommittedEvent event_one;
        public static CommittedEvent event_two;
        public static CommittedEvent event_three;

        Establish context = () =>
        {
            event_one = new CommittedEvent(0, DateTimeOffset.Now, correlation_id, microservice_id, tenant_id, cause, event_a_artifact, "one");
            event_two = new CommittedEvent(1, DateTimeOffset.Now, correlation_id, microservice_id, tenant_id, cause, event_a_artifact, "two");
            event_three = new CommittedEvent(2, DateTimeOffset.Now, correlation_id, microservice_id, tenant_id, cause, event_b_artifact, "three");
        };
    }
}
