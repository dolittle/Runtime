// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_unregistering
{
    public class and_there_is_a_filter_registered_for_stream : given.all_dependencies
    {
        static StreamId source_stream;
        static StreamId target_stream;
        static Exception exception;

        Establish context = () =>
        {
            source_stream = Guid.NewGuid();
            target_stream = Guid.NewGuid();
            var filter_processor = new Moq.Mock<IFilterProcessor<RemoteFilterDefinition>>();
            filter_processor.SetupGet(_ => _.Definition).Returns(new RemoteFilterDefinition(source_stream, target_stream));
            filter_registry.Register(filter_processor.Object).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => filter_registry.Unregister(target_stream));

        It should_not_throw_any_exceptions = () => exception.ShouldBeNull();
    }
}