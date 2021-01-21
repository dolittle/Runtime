// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Protobuf.for_Serializer
{
    public class when_serializing_concept_of_integer : given.a_serializer
    {
        class type_for_serialization
        {
            public ConceptAs<int> concept { get; set; }
        }

        static type_for_serialization original;
        static type_for_serialization deserialized;

        Establish context = () =>
        {
            original = new type_for_serialization { concept = new ConceptAs<int> { Value = int.MaxValue } };
            message_descriptions.Setup(_ => _.GetFor<type_for_serialization>()).Returns(MessageDescription.DefaultFor<type_for_serialization>());
        };

        Because of = () =>
        {
            var bytes = serializer.ToProtobuf(original);
            deserialized = serializer.FromProtobuf<type_for_serialization>(bytes);
        };

        It should_hold_the_correct_value = () => deserialized.concept.ShouldEqual(original.concept);
    }
}