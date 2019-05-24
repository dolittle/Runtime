/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_EventSourceExtensions
{
    public class when_converting_versioned_event_source_to_and_from_protobuf
    {
        static VersionedEventSource original;
        static Runtime.Grpc.Interaction.Protobuf.VersionedEventSource protobuf;
        static VersionedEventSource result;

        Establish context = () => original = new VersionedEventSource(
            new EventSourceVersion(42,43),
            new EventSourceKey(EventSourceId.New(),ArtifactId.New())
        );

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToVersionedEventSource();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}