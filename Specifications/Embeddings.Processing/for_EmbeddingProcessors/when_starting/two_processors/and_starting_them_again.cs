// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Embeddings.Store;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors.when_starting.two_processors;

public class and_starting_them_again : given.two_tenants_and_processors
{
    static EmbeddingId embedding;
    static Task<Try> started_processors;

    Establish context = () =>
    {
        var processor_a = new Mock<IEmbeddingProcessor>();
        processor_a.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>(async (cancellationToken) =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
            return Try.Succeeded();

        });

        var processor_b = new Mock<IEmbeddingProcessor>();
        processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>(async (cancellationToken) =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
            return Try.Succeeded();

        });

        factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
        factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

        embedding = "801034ab-9963-48b8-b27b-d3abb01bae2a";
        started_processors = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None);
    };

    static Try result;
    Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None).GetAwaiter().GetResult();

    It should_not_be_successful = () => result.Success.ShouldBeFalse();
    It should_fail_because_emmbedding_processors_already_registered = () => result.Exception.ShouldBeOfExactType<EmbeddingProcessorsAlreadyRegistered>();
    It should_still_have_old_processors_not_be_cancelled = () => started_processors.ShouldStillBeRunning();
}