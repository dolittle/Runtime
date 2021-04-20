// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors.when_starting.two_processors
{
    public class and_one_processor_fails : given.two_tenants_and_processors
    {
        static Exception exception;
        static CancellationToken processor_a_cancellation_token;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            var processor_a = new Mock<IEmbeddingProcessor>();
            processor_a.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>(async (cancellationToken) =>
            {
                processor_a_cancellation_token = cancellationToken;
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }
                return Try.Succeeded();

            });

            exception = new Exception();
            var processor_b = new Mock<IEmbeddingProcessor>();
            processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<Try>(exception));

            factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

            embedding = "e7c1fe4e-4f84-493b-9a98-b6419abc76c8";
        };

        static Try result;
        Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None).GetAwaiter().GetResult();

        It should_have_failed = () => result.Success.ShouldBeFalse();
        It should_return_the_thrown_exception = () => result.Exception.ShouldBeTheSameAs(exception);
        It should_cancel_the_other_processor = () => processor_a_cancellation_token.IsCancellationRequested.ShouldBeTrue();
    }
}