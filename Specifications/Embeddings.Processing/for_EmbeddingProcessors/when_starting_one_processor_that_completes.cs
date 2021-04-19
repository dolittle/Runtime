// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Embeddings.Store;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors
{
    public class when_starting_one_processor_that_completes : given.two_tenants_and_processors
    {
        static CancellationTokenSource cts;
        static CancellationToken processor_a_cancellation_token;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var processor_a = new Mock<IEmbeddingProcessor>();
            processor_a.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>((cancellationToken) =>
            {
                processor_a_cancellation_token = cancellationToken;

                return new TaskCompletionSource<Try>().Task;
            });

            var processor_b = new Mock<IEmbeddingProcessor>();
            processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(Try.Succeeded()));

            factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

            embedding = "c0b4c09b-00e4-4974-a74f-980b33b59758";
            // while (!System.Diagnostics.Debugger.IsAttached) System.Threading.Thread.Sleep(50);
        };

        static Try result;
        Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, cts.Token).GetAwaiter().GetResult();

        It should_be_successful = () => result.Success.ShouldBeTrue();
        It should_cancel_the_other_processor = () => processor_a_cancellation_token.IsCancellationRequested.ShouldBeTrue();
    }
}