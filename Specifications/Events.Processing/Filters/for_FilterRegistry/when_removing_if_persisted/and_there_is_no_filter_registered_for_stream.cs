// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_removing_if_persisted
{
    public class and_there_is_no_filter_registered_for_stream : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => filter_registry.RemoveIfPersisted(Guid.NewGuid()).GetAwaiter().GetResult());

        It should_fail_because_there_are_no_filter_registered_for_stream = () => exception.ShouldBeOfExactType<NoFilterRegisteredForStream>();
    }
}