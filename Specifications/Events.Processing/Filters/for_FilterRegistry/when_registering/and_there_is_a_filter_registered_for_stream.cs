// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_registering
{
    public class and_there_is_a_filter_registered_for_stream : given.all_dependencies
    {
        static Moq.Mock<IFilterProcessor<RemoteFilterDefinition>> filter_processor;
        static Exception exception;

        Establish context = () =>
        {
            filter_processor = new Moq.Mock<IFilterProcessor<RemoteFilterDefinition>>();
            filter_processor.SetupGet(_ => _.Definition).Returns(new RemoteFilterDefinition(Guid.NewGuid(), Guid.NewGuid()));
            filter_registry.Register(filter_processor.Object).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => filter_registry.Register(filter_processor.Object).GetAwaiter().GetResult());

        It should_fail_because_filter_for_stream_has_already_been_registered = () => exception.ShouldBeOfExactType<FilterForStreamAlreadyRegistered>();
    }
}