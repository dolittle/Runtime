// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamPositionRange;

public class when_creating_range
{
    static StreamPosition from;
    static uint range_length;
    static StreamPositionRange range;

    Establish context = () =>
    {
        from = 0;
        range_length = 1;
    };

    Because of = () => range = new StreamPositionRange(from, range_length);

    It should_have_the_correct_from_position = () => range.From.ShouldEqual(from);
    It should_have_the_correct_length = () => range.Length.ShouldEqual(range.Length);
}