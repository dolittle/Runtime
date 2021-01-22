// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Time.when_converting
{
    [Subject(typeof(DateTimeExtensions))]
    public class from_to_date_time_to_unix_timestamp
    {
        static long now_in_milliseconds;
        static long utc_now_in_milliseconds;
        static long converted_unix_timestamp;
        static DateTimeOffset now;

        Establish context = () =>
        {
            now = DateTimeOffset.Now;
            now_in_milliseconds = now.ToUnixTimeMilliseconds();
            utc_now_in_milliseconds = now.ToUniversalTime().ToUnixTimeMilliseconds();
        };

        Because of = () => converted_unix_timestamp = now.DateTime.ToUnixTimeMilliseconds();

        It should_convert_the_time = () => converted_unix_timestamp.ShouldEqual(now_in_milliseconds);
        It should_convert_time_and_local_time_to_the_same_timestamp = () => converted_unix_timestamp.ShouldEqual(utc_now_in_milliseconds);
    }
}