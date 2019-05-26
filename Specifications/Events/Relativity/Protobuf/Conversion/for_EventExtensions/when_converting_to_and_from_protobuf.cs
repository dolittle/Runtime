/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_EventExtensions
{
    public class when_converting_to_and_from_protobuf
    {
        static EventMetadata original;
        static Runtime.Grpc.Interaction.EventMetadata protobuf;
        static Dolittle.Runtime.Events.EventMetadata result;

        Establish context = () => 
        {
            var artifactId = ArtifactId.New();
            original = new EventMetadata(
                EventId.New(),
                new VersionedEventSource(EventSourceId.New(), artifactId),
                CorrelationId.New(),
                new Dolittle.Artifacts.Artifact(artifactId, ArtifactGeneration.First),
                DateTimeOffset.FromUnixTimeMilliseconds(1540715541241),
                new OriginalContext(
                    Application.New(), 
                    BoundedContext.New(), 
                    Guid.NewGuid(),
                    "Development",
                    new Dolittle.Security.Claims(
                        new[] {
                            new Dolittle.Security.Claim("FirstClaim","FirstValue","FirstClaimType"),
                            new Dolittle.Security.Claim("SecondClaim","SecondValue","SecondClaimType")
                        }
                    ),
                    new CommitSequenceNumber(42))
            );
        };

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToEventMetadata();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}