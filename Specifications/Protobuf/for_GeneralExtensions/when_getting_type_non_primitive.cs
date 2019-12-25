// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_non_primitive
    {
        class my_thing { }

        static Types type;

        Because of = () => type = new my_thing().GetProtobufType();

        It should_be_unknown = () => type.ShouldEqual(Types.Unknown);
    }
}