// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_PersistProjectionDefinitionForAllTenants.when_persisting_definition.and_there_are_multiple_tenants;

public class and_persisting_fails_for_one_tenant : given.all_dependencies
{
    static EmbeddingId embedding;
    static EmbeddingDefinition definition;
    static PersistEmbeddingDefinitionForAllTenants persister;
    static Exception exception;

    Establish context = () =>
    {
        exception = new Exception();
        embedding = "72f234da-6f88-4e78-87a5-bfb34bc51096";
        dynamic state = new JObject();
        state.Hello = "world";
        definition = new EmbeddingDefinition(
            embedding,
            new[] { new Artifact("250fefb1-c99b-4d37-98ab-a46621633329", 1) },
            JsonConvert.SerializeObject(state));
        var definitions = new Moq.Mock<IEmbeddingDefinitions>();
        var faulty_definitions = new Moq.Mock<IEmbeddingDefinitions>();
        definitions
            .Setup(_ => _.TryPersist(definition, IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<bool>.Succeeded(true))); ;
        faulty_definitions
            .Setup(_ => _.TryPersist(definition, IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<bool>.Failed(exception))); ;
        persister = WithTenants(_ =>
            _.ForTenant("923877a3-2836-41a8-8d54-c3945b68868b", definitions)
                .ForTenant("eadd890f-44d2-432a-90c3-a9ce7eafdbe0", faulty_definitions));
    };
    static Try result;
    Because of = () => result = persister.TryPersist(definition, CancellationToken.None).GetAwaiter().GetResult();

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_return_the_correct_exception = () => result.Exception.ShouldEqual(exception);

}