// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionDefinitionsForAllTenants.when_comparing_definitions.and_there_is_one_tenant;

public class and_event_type_generation_differs : given.one_tenant
{
    static EmbeddingId embedding;
    static EmbeddingDefinition persisted_definition;
    static EmbeddingDefinition new_definition;

    Establish context = () =>
    {
        embedding = "72f234da-6f88-4e78-87a5-bfb34bc51096";
        var event_type = new Artifact("a3bc822b-c693-4f17-9707-ff4764861391", 1);
        dynamic persisted_state = new JObject();
        persisted_state.Hello = "world";
        persisted_definition = new EmbeddingDefinition(
            embedding,
            new[] { event_type },
            JsonConvert.SerializeObject(persisted_state));
        new_definition = persisted_definition with { Events = new[] { event_type with { Generation = 5 } } };
        definitions
            .Setup(_ => _.TryGet(embedding, IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(persisted_definition)));

    };
    static IDictionary<TenantId, EmbeddingDefinitionComparisonResult> results;
    Because of = () => results = comparer.DiffersFromPersisted(new_definition, CancellationToken.None).GetAwaiter().GetResult();

    It should_only_contain_result_for_tenant = () => results.Keys.ShouldContainOnly(tenant);
    It should_be_a_successful_result = () => results[tenant].Succeeded.ShouldBeTrue();

}