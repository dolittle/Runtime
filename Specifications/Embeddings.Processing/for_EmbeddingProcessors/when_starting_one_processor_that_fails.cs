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

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors
{
    public class when_starting_one_processor_that_fails : given.two_tenants_and_processors
    {
        static Exception exception;
        static CancellationToken processor_a_cancellation_token;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            var processor_a = new Mock<IEmbeddingProcessor>();
            processor_a.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns<CancellationToken>((cancellationToken) =>
            {
                processor_a_cancellation_token = cancellationToken;
                return new TaskCompletionSource<Try>().Task;

            });

            exception = new Exception();
            var processor_b = new Mock<IEmbeddingProcessor>();
            processor_b.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<Try>(exception));

            factory.Setup(_ => _(tenant_a)).Returns(processor_a.Object);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b.Object);

            embedding = "e7c1fe4e-4f84-493b-9a98-b6419abc76c8";
        };

        static Task<Try> result;
        Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None);

        It should_have_faulted_task = () => result.Status.ShouldEqual(TaskStatus.Faulted);
        It should_have_failed = () => result.Result.Success.ShouldBeFalse();
        It should_return_the_thrown_exception = () => result.Result.Exception.ShouldBeTheSameAs(exception);
        It should_cancel_the_other_processor = () => processor_a_cancellation_token.IsCancellationRequested.ShouldBeTrue();
    }
}