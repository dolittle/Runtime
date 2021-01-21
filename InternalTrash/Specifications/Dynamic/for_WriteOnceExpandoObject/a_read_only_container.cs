// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Immutability;
using Machine.Specifications;

namespace Dolittle.Dynamic.Specs.for_WriteOnceExpandoObject
{
    [Behaviors]
    public class a_read_only_container
    {
        protected static Exception exception;
        It should_throw_a_read_only_object_exception = () => exception.ShouldBeOfExactType<CannotWriteToAnImmutable>();
    }
}
