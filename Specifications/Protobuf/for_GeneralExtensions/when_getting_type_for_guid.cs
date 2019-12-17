// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_guid
    {
        static Types type;

        Because of = () => type = Guid.NewGuid().GetProtobufType();

        It should_be_guid = () => type.ShouldEqual(Types.Guid);
    }
}