// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessorFactory;

public class when_starting_two_processors : given.all_dependencies
{
    static TenantId tenant;
    static EmbeddingId embedding_id;
    static IEmbedding embedding;
    static ProjectionState initial_state;

    Establish context = () =>
    {
        tenant = "4e83c415-f902-4ae1-bed8-8a06e0658ac7";
        embedding_id = "f4a3459e-98f1-4bc2-80c6-549f3492b49e";
        embedding = Mock.Of<IEmbedding>();
        initial_state = "some state";
    };

    static IEmbeddingProcessor result;
    Because of = () => result = factory.Create(tenant, embedding_id, embedding, initial_state);

    It should_create_an_embedding_processor = () => result.ShouldNotBeNull();
    It should_set_the_correct_tenant = () => execution_context_manager.Verify(_ => _.CurrentFor(tenant, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()), Times.Once);
    It should_revert_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()), Times.Once);
    It should_get_the_execution_context = () => execution_context_manager.Verify(_ => _.Current, Times.Once);
    It should_not_do_anything_else_with_execution_context = () => execution_context_manager.VerifyNoOtherCalls();
}