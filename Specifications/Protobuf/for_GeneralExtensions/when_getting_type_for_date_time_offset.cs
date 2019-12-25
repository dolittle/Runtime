// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_date_time_offset
    {
        static Types type;

        Because of = () => type = DateTimeOffset.Now.GetProtobufType();

        It should_be_date_time_offst = () => type.ShouldEqual(Types.DateTimeOffset);
    }
}