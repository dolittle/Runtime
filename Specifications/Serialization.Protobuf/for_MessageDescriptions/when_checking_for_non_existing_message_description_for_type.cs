// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions;

public class when_checking_for_non_existing_message_description_for_type : given.no_message_descriptions
{
    static bool result;
    Because of = () => result = message_descriptions.HasFor<class_with_properties>();

    It should_not_have_a_description = () => result.ShouldBeFalse();
}