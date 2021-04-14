// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.for_EmbeddingProcessors
{
    public class when_starting_one_processor_that_fails : given.two_tenants
    {
        static EmbeddingProcessors processors;
        static Exception exception;
        static Mock<EmbeddingProcessorFactory> factory;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            processors = new EmbeddingProcessors(tenants.Object, Mock.Of<ILogger>());

            var processor_a = new Mock<IEmbeddingProcessor>();
            processor_a.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(Task.Delay(Timeout.Infinite));

            exception = new Exception();
            var processor_b = new Mock<IEmbeddingProcessor>();
            processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromException(exception));

            factory = new Mock<EmbeddingProcessorFactory>();
            factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

            embedding = "c0b4c09b-00e4-4974-a74f-980b33b59758";
        };

        static Task completed;
        Because of = () => completed = processors.StartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None);

        It should_have_failed = () => completed.Status.ShouldEqual(TaskStatus.Faulted);
        It should_return_the_thrown_exception = () => completed.Exception.ShouldBeTheSameAs(exception);
    }
}