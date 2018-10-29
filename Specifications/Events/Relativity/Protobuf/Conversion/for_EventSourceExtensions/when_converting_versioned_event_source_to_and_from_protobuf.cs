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
        static Dolittle.Runtime.Events.VersionedEventSource original;
        static VersionedEventSource protobuf;
        static Dolittle.Runtime.Events.VersionedEventSource result;

        Establish context = () => original = new Dolittle.Runtime.Events.VersionedEventSource(
            new Dolittle.Runtime.Events.EventSourceVersion(42,43),
            EventSourceId.New(),
            ArtifactId.New()
        );

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToVersionedEventSource();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}