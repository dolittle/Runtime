// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions
{
    public class when_registering_message_description_for_same_type_twice : given.no_message_descriptions
    {
        static MessageDescription message_description;
        static Exception result;
        Establish context = () => message_description = MessageDescription.DefaultFor<class_with_properties>();

        Because of = () =>
        {
            message_descriptions.SetFor<class_with_properties>(message_description);
            result = Catch.Exception(() => message_descriptions.SetFor<class_with_properties>(message_description));
        };

        It should_throw_message_description_already_registered_for_type = () => result.ShouldBeOfExactType<MessageDescriptionAlreadyRegisteredForType>();
    }
}