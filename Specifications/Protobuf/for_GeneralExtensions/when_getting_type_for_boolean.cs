// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_boolean
    {
        static Types type;

        Because of = () => type = true.GetProtobufType();

        It should_be_boolean = () => type.ShouldEqual(Types.Boolean);
    }
}