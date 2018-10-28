/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_EventSourceExtensions
{
    public class when_converting_event_source_version_to_and_from_protobuf
    {
        static Dolittle.Runtime.Events.EventSourceVersion original;
        static EventSourceVersion protobuf;
        static Dolittle.Runtime.Events.EventSourceVersion result;

        Establish context = () => original = new Dolittle.Runtime.Events.EventSourceVersion(42,43);

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToEventSourceVersion();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }    
}