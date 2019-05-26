/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Reflection;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_ArtifactExtensions
{
    public class when_converting_committed_event_stream_to_and_from_protobuf
    {
        static CommittedEventStream original;
        static Runtime.Grpc.Interaction.CommittedEventStream protobuf;
        static CommittedEventStream result;

        Establish context = () => 
        {
            var artifactId = ArtifactId.New();
            var eventSource = new VersionedEventSource(EventSourceId.New(), artifactId);
            var eventMetadata = new EventMetadata(
                EventId.New(),
                eventSource,
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

            original = new CommittedEventStream(
                new CommitSequenceNumber(42),
                eventSource,
                CommitId.New(),
                CorrelationId.New(),
                DateTimeOffset.FromUnixTimeMilliseconds(1540715541241),
                new EventStream(new[] {
                    new EventEnvelope(
                        eventMetadata,
                        new Dolittle.PropertyBags.PropertyBag(
                            new NullFreeDictionary<string, object> {
                                {"string","a string"},
                                {"int", 42},
                                {"long", 42L}
                            }))
                })

            );
        };

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToCommittedEventStream();
        };

        It should_be_equal_to_the_original = () => 
            typeof(CommittedEventStream)
                .GetProperties(BindingFlags.Instance|BindingFlags.Public)
                .ForEach(_ => 
                    _.GetValue(result).ShouldEqual(_.GetValue(original)));
    }
}