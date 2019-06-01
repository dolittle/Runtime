/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Collections;
using Dolittle.PropertyBags;
using Dolittle.Time;
using Google.Protobuf.Collections;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_PropertyBagExtensions
{
    public class when_converting_property_bag_with_all_supported_types_to_and_from_protobuf
    {
        static Dolittle.PropertyBags.PropertyBag original;
        static PropertyBag protobuf;
        static Dolittle.PropertyBags.PropertyBag result;

        Establish context = () => original = new Dolittle.PropertyBags.PropertyBag(
            new NullFreeDictionary<string, object> {
                {"string","a string"},
                {"int", 42},
                {"long", 42L},
                {"uint", (uint)42},
                {"ulong", (ulong)42L},
                {"int32", (Int32)42},
                {"int64", (Int64)42L},
                {"uint32", (UInt32)42},
                {"uint64", (UInt64)42L},
                {"float", 42f},
                {"double", 42d},
                {"decimal", 42d},
                {"bool", true},
                {"dateTime", DateTimeOffset.FromUnixTimeMilliseconds(1540715532995).UtcDateTime},
                {"dateTimeOffset", DateTimeOffset.FromUnixTimeMilliseconds(1540715541241) },
                {"guid", Guid.NewGuid()}
            }
        );

        Because of = () =>
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToPropertyBag();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}