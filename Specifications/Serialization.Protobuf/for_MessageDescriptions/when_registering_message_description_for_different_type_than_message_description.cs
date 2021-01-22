// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions
{
    public class when_registering_message_description_for_different_type_than_message_description : given.no_message_descriptions
    {
        static MessageDescription message_description;
        static Exception result;

        Establish context = () => message_description = MessageDescription.DefaultFor<class_with_properties>();

        Because of = () => result = Catch.Exception(() => message_descriptions.SetFor<string>(message_description));

        It should_throw_type_mismatch_for_message_description = () => result.ShouldBeOfExactType<TypeMismatchForMessageDescription>();
    }
}