// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Collections;
using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_PropertyBagExtensions
{
    public class when_converting_property_bag_with_all_supported_types_to_and_from_protobuf
    {
        static PropertyBags.PropertyBag original;
        static PropertyBag protobuf;
        static PropertyBags.PropertyBag result;

        Establish context = () => original = new PropertyBags.PropertyBag(
            new NullFreeDictionary<string, object>
            {
                { "string", "a string" },
                { "int", 42 },
                { "long", 42L },
                { "uint", 42U },
                { "ulong", 42UL },
                { "float", 42f },
                { "double", 42d },
                { "decimal", 42d },
                { "bool", true },
                { "dateTime", DateTimeOffset.FromUnixTimeMilliseconds(1540715532995).UtcDateTime },
                { "dateTimeOffset", DateTimeOffset.FromUnixTimeMilliseconds(1540715541241) },
                { "guid", Guid.NewGuid() }
            });

        Because of = () =>
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToPropertyBag();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}