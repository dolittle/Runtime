// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Heads.for_ConnectedHeads.given
{
    public class two_heads
    {
        protected static Head first_head;
        protected static Head second_head;

        Establish context = () =>
        {
            first_head = new Head(Guid.NewGuid(), "first head", 42, "Some runtime", new[] { "first", "second" }, DateTimeOffset.UtcNow);
            second_head = new Head(Guid.NewGuid(), "second head", 43, "Some other runtime", new[] { "third", "fourth" }, DateTimeOffset.UtcNow.AddDays(5));
        };
    }
}