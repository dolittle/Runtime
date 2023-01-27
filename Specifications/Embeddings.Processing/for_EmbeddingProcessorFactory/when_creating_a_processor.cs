// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store.State;
using FluentAssertions;
using Machine.Specifications;
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
    Because of = () => result = factory.Create(embedding_id, embedding, initial_state, execution_context);

    It should_create_an_embedding_processor = () => result.Should().NotBeNull();
}