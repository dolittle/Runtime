// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions;

public class when_registering_message_description_for_type : given.no_message_descriptions
{
    static MessageDescription message_description;
    Establish context = () => message_description = MessageDescription.DefaultFor<class_with_properties>();

    Because of = () => message_descriptions.SetFor<class_with_properties>(message_description);

    It should_consider_having_it = () => message_descriptions.HasFor<class_with_properties>();
    It should_return_it_when_asking_for_it = () => message_descriptions.GetFor<class_with_properties>().ShouldEqual(message_description);
}