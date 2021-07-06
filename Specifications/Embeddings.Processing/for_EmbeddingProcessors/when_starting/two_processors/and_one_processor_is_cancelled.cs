// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Embeddings.Store;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors.when_starting.two_processors
{
    public class and_one_processor_is_cancelled : given.two_tenants_and_processors
    {
        static CancellationTokenSource cts;
        static CancellationToken processor_a_cancellation_token;
        static CancellationToken processor_b_cancellation_token;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
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

            var processor_b = new Mock<IEmbeddingProcessor>();
            processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>(async (cancellationToken) =>
            {
                processor_b_cancellation_token = cancellationToken;
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }
                return Try.Succeeded();

            });

            factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

            embedding = "801034ab-9963-48b8-b27b-d3abb01bae2a";
        };

        static Try result;
        Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, cts.Token).GetAwaiter().GetResult();

        It should_be_successful = () => result.Success.ShouldBeTrue();
        It should_cancel_the__processor = () => processor_a_cancellation_token.IsCancellationRequested.ShouldBeTrue();
        It should_cancel_the_other_processor = () => processor_b_cancellation_token.IsCancellationRequested.ShouldBeTrue();
        It should_have_unregistered_processors = () => processors.HasEmbeddingProcessors(embedding).ShouldBeFalse();
    }
}