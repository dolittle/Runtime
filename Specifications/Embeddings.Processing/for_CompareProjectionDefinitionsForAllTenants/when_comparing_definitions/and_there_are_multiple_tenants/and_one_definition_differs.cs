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
    public class and_one_definition_differs : given.all_dependencies
    {
        static EmbeddingId embedding;
        static EmbeddingDefinition definition;
        static CompareEmbeddingDefinitionsForAllTenants comparer;
        static TenantId differing_tenant;

        Establish context = () =>
        {
            differing_tenant = "c16f18a8-aeae-41f6-bef4-456a29359ee9";
            embedding = "72f234da-6f88-4e78-87a5-bfb34bc51096";
            dynamic state = new JObject();
            state.Hello = "world";
            definition = new EmbeddingDefinition(
                embedding,
                new[] { new Artifact("250fefb1-c99b-4d37-98ab-a46621633329", 1) },
                JsonConvert.SerializeObject(state));
            var definitions = new Moq.Mock<IEmbeddingDefinitions>();
            var differing_definitions = new Moq.Mock<IEmbeddingDefinitions>();
            definitions
                .Setup(_ => _.TryGet(embedding, IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(definition)));
            differing_definitions
                .Setup(_ => _.TryGet(embedding, IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(definition with { Embedding = "012e0c03-b4e0-4efe-a81a-1c5788e8dc2a" })));
            comparer = WithTenants(_ =>
                _.ForTenant("43f89597-35a4-45db-a008-c156468bfcef", definitions)
                .ForTenant(differing_tenant, differing_definitions));
        };
        static IDictionary<TenantId, EmbeddingDefinitionComparisonResult> results;
        Because of = () => results = comparer.DiffersFromPersisted(definition, CancellationToken.None).GetAwaiter().GetResult();

        It should_only_contain_result_for_configured_tenants = () => results.Keys.ShouldContainOnly(tenants.All);
        It should_have_unsuccessful_results = () => results.Any(_ => !_.Value.Succeeded).ShouldBeTrue();
        It should_have_the_correct_differing_tenant = () => results.Single(_ => !_.Value.Succeeded).Key.ShouldEqual(differing_tenant);

    }
}