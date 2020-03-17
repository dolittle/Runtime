// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.when_registering
{
    public class and_validation_fails : given.all_dependencies
    {
        static Moq.Mock<IFilterProcessor<RemoteFilterDefinition>> filter_processor;
        static Exception exception;

        Establish context = () =>
        {
            filter_processor = new Moq.Mock<IFilterProcessor<RemoteFilterDefinition>>();
            filter_processor.SetupGet(_ => _.Definition).Returns(new RemoteFilterDefinition(Guid.NewGuid(), Guid.NewGuid()));
            validators.Setup(_ => _.Validate(Moq.It.IsAny<IFilterProcessor<RemoteFilterDefinition>>(), Moq.It.IsAny<CancellationToken>())).Throws(new Exception());
        };

        Because of = () => exception = Catch.Exception(() => filter_registry.Register(filter_processor.Object).GetAwaiter().GetResult());

        It should_fail_because_validation_fails = () => exception.ShouldBeOfExactType<Exception>();
    }
}