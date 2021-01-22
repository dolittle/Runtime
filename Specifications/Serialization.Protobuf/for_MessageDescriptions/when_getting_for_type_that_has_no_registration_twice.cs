// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions
{
    public class when_getting_for_type_that_has_no_registration_twice : given.no_message_descriptions
    {
        static MessageDescription first;
        static MessageDescription second;

        Because of = () =>
        {
            first = message_descriptions.GetFor<class_with_properties>();
            second = message_descriptions.GetFor<class_with_properties>();
        };

        It should_return_same_instance = () => first.GetHashCode().ShouldEqual(second.GetHashCode());
    }
}