// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions;

public class when_getting_for_type_that_has_no_registration : given.no_message_descriptions
{
    static MessageDescription result;

    Because of = () => result = message_descriptions.GetFor<class_with_properties>();

    It should_return_a_default_description = () => result.ShouldNotBeNull();
}