// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Time.when_converting
{
    [Subject(typeof(DateTimeExtensions))]
    public class from_milliseconds_to_date_time
    {
        static long source_in_milliseconds;
        static DateTime converted;
        static DateTimeOffset now;

        Establish context = () =>
        {
            now = DateTimeOffset.Now;
            source_in_milliseconds = now.ToUnixTimeMilliseconds();
        };

        Because of = () => converted = source_in_milliseconds.ToDateTime();

        It should_convert_the_time = () => converted.LossyEquals(now.ToUniversalTime()).ShouldBeTrue();
        It should_create_the_datetime_as_utc = () => converted.Kind.ShouldEqual(DateTimeKind.Utc);
    }
}