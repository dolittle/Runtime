// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Streams.for_StreamPositionRange
{
    public class when_creating_range
    {
        static StreamPosition from;
        static StreamPosition to;
        static StreamPositionRange range;

        Establish context = () =>
        {
            from = 0;
            to = 1;
        };

        Because of = () => range = new StreamPositionRange(from, to);

        It should_have_the_correct_from_position = () => range.From.ShouldEqual(from);
        It should_have_the_correct_to_position = () => range.To.ShouldEqual(to);
    }
}