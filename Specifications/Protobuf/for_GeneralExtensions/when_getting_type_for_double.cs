// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_double
    {
        static Types type;

        Because of = () => type = 42d.GetProtobufType();

        It should_be_double = () => type.ShouldEqual(Types.Double);
    }
}