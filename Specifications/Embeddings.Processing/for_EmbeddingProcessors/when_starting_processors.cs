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
    public class when_starting_processors : given.two_tenants_and_processors
    {
        static IEmbeddingProcessor processor_a;
        static IEmbeddingProcessor processor_b;
        static EmbeddingId embedding;

        Establish context = () =>
        {
            var processor_a_mock = new Mock<IEmbeddingProcessor>();
            processor_a_mock.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(new TaskCompletionSource<Try>().Task);
            processor_a = processor_a_mock.Object;

            var processor_b_mock = new Mock<IEmbeddingProcessor>();
            processor_b_mock.Setup(_ => _.Start(Moq.It.IsAny<CancellationToken>())).Returns(new TaskCompletionSource<Try>().Task);
            processor_b = processor_b_mock.Object;

            factory.Setup(_ => _(tenant_a)).Returns(processor_a);
            factory.Setup(_ => _(tenant_b)).Returns(processor_b);

            embedding = "f4a3459e-98f1-4bc2-80c6-549f3492b49e";
        };

        static Task<Try> result;
        Because of = () => result = processors.TryStartEmbeddingProcessorForAllTenants(embedding, factory.Object, CancellationToken.None);

        It should_get_tenants = () => tenants.Verify(_ => _.All);
        It should_create_a_processor_for_tenant_a = () => factory.Verify(_ => _(tenant_a));
        It should_create_a_processor_for_tenant_b = () => factory.Verify(_ => _(tenant_b));
        It should_not_create_processors_for_any_other_tenants = () => factory.VerifyNoOtherCalls();
        It should_have_processors_for_the_embedding = () => processors.HasEmbeddingProcessors(embedding).ShouldBeTrue();
        It should_return_processor_a_for_tenant_a = () =>
        {
            processors.TryGetEmbeddingProcessorFor(tenant_a, embedding, out var processor).ShouldBeTrue();
            processor.ShouldBeTheSameAs(processor_a);
        };
        It should_return_processor_b_for_tenant_b = () =>
        {
            processors.TryGetEmbeddingProcessorFor(tenant_b, embedding, out var processor).ShouldBeTrue();
            processor.ShouldBeTheSameAs(processor_b);
        };
        It should_not_have_a_processor_for_another_tenant = () => processors.TryGetEmbeddingProcessorFor("621aca1c-eddf-4ed4-a2ca-512d9a49d746", embedding, out var _).ShouldBeFalse();
        It should_not_have_processors_for_another_embedding = () => processors.HasEmbeddingProcessors("f9bd3173-b7a8-4081-ab0d-61351e6a9cb5").ShouldBeFalse();
        It should_be_running = () => result.Status.ShouldEqual(TaskStatus.Running);
    }
}