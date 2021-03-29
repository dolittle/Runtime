// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_CompareProjectionDefinitionsForAllTenants.when_there_is_one_tenant
{
    public class and_definition_is_not_persisted : given.all_dependencies
    {
        static TenantId tenant;
        static ProjectionDefinition definition;
        static IDictionary<TenantId, ProjectionDefinitionComparisonResult> result;
        static ProjectionKey key;

        Establish context = () =>
        {
            tenant = "8f670924-d7f5-45f3-8f75-922ca86cca8c";
            tenants.Setup(_ => _.All).Returns(new ObservableCollection<TenantId>(new[] { tenant }));
            definition = given.projection_definition_builder
                .create(
                    "c3c7c90e-b8e3-41eb-b641-1dff6fe90777",
                    "5e1c13f3-4af4-4335-93ef-7612b67f0f67")
                .with_selector(new ProjectionEventSelector(new Artifact(Guid.NewGuid(), 1), ProjectEventKeySelectorType.EventSourceId))
                .build();

            definitions
                .Setup(_ => _.TryGet(definition.Projection, definition.Scope, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<ProjectionDefinition>>(false));
        };
        Because of = () => result = comparer.DiffersFromPersisted(definition, CancellationToken.None).GetAwaiter().GetResult();

        It should_have_result_for_tenant = () => result.ContainsKey(TenantId.Development);
        It should_be_a_successful_result = () => result[tenant].Succeeded.ShouldBeTrue();
    }
}