// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_Serializer.given;

public class a_serializer : all_dependencies
{
    protected static Serializer serializer;

    Establish context = () => serializer = new Serializer(message_descriptions.Object, value_converters.Object);
}