// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionDefinitionsForAllTenants.when_comparing_definitions.and_there_are_multiple_tenants
{
    public class all_with_the_same_definitions : given.all_dependencies
    {
        static EmbeddingId embedding;
        static EmbeddingDefinition definition;
        static CompareEmbeddingDefinitionsForAllTenants comparer;

        Establish context = () =>
        {
            embedding = "72f234da-6f88-4e78-87a5-bfb34bc51096";
            dynamic state = new JObject();
            state.Hello = "world";
            definition = new EmbeddingDefinition(
                embedding,
                new[] { new Artifact("250fefb1-c99b-4d37-98ab-a46621633329", 1) },
                JsonConvert.SerializeObject(state));
            var definitions = new Moq.Mock<IEmbeddingDefinitions>();
            definitions
                .Setup(_ => _.TryGet(embedding, IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(definition))); ;
            comparer = WithTenants(_ =>
                _.ForTenant("923877a3-2836-41a8-8d54-c3945b68868b", definitions)
                .ForTenant("eadd890f-44d2-432a-90c3-a9ce7eafdbe0", definitions));
        };
        static IDictionary<TenantId, EmbeddingDefinitionComparisonResult> results;
        Because of = () => results = comparer.DiffersFromPersisted(definition, CancellationToken.None).GetAwaiter().GetResult();

        It should_only_contain_result_for_configured_tenants = () => results.Keys.ShouldContainOnly(tenants.All);
        It should_have_no_unsuccessful_results = () => results.Any(_ => !_.Value.Succeeded).ShouldBeFalse();

    }
}