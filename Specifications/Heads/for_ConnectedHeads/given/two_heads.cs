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
            first_head = new Head(Guid.NewGuid(), "Some runtime", DateTimeOffset.UtcNow);
            second_head = new Head(Guid.NewGuid(), "Some other runtime", DateTimeOffset.UtcNow.AddDays(5));
        };
    }
}