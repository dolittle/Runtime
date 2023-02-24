// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.given;

public class all_dependencies
{
    protected static Type filter_validator_type;
    protected static TenantId tenant_id;
    protected static ScopeId scope_id;
    protected static StreamId filter_source_stream;
    protected static StreamId filter_target_stream;
    protected static StreamProcessorId stream_processor_id;
    protected static FilterDefinition filter_definition;
    protected static TypeFilterWithEventSourcePartitionDefinition different_filter_definition;
    protected static IFilterProcessor<FilterDefinition> filter_processor;
    protected static Mock<ICanValidateFilterFor<FilterDefinition>> filter_validator;
    protected static StreamProcessorState stream_processor_state;
    
    // protected static Mock<IContainer> container;
    protected static Mock<IStreamProcessorStates> stream_processor_state_repository;
    protected static Mock<IFilterDefinitions> filter_definitions;
    protected static Mock<ICompareFilterDefinitions> definition_comparer;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        tenant_id = "b079237c-487d-4614-b20c-c64e95646bc4";
        var mocks = new MockRepository(MockBehavior.Strict);

        filter_validator_type = typeof(all_dependencies);
        filter_validator = mocks.Create<ICanValidateFilterFor<FilterDefinition>>();

        scope_id = Guid.Parse("64c358dc-b174-4e97-908a-bc900f200974");

        filter_source_stream = StreamId.EventLog;
        filter_target_stream = Guid.Parse("a99d008d-54f9-48ad-ac63-8af52def20bd");
        stream_processor_id = new StreamProcessorId(scope_id, filter_target_stream.Value, filter_source_stream);
        filter_definition = new FilterDefinition(filter_source_stream, filter_target_stream, false);
        different_filter_definition = new TypeFilterWithEventSourcePartitionDefinition(filter_source_stream, filter_target_stream, Array.Empty<ArtifactId>(), true);

        var filter_processor_mock = mocks.Create<IFilterProcessor<FilterDefinition>>();
        filter_processor_mock
            .SetupGet(_ => _.Definition)
            .Returns(filter_definition);
        filter_processor_mock
            .SetupGet(_ => _.Scope)
            .Returns(scope_id);
        filter_processor_mock
            .SetupGet(_ => _.Identifier)
            .Returns(filter_target_stream.Value);
        filter_processor = filter_processor_mock.Object;

        filter_validator = mocks.Create<ICanValidateFilterFor<FilterDefinition>>();

        stream_processor_state = new StreamProcessorState(10, 13, DateTimeOffset.Now);

        stream_processor_state_repository = mocks.Create<IStreamProcessorStates>();
        stream_processor_state_repository
            .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
            .Returns(Task.FromResult(Try<IStreamProcessorState>.Succeeded(stream_processor_state)));

        filter_definitions = mocks.Create<IFilterDefinitions>();
        filter_definitions
            .Setup(_ => _.TryGetFromStream(scope_id, filter_target_stream, cancellation_token))
            .Returns(Task.FromResult(Try<IFilterDefinition>.Succeeded(filter_definition)));

        definition_comparer = mocks.Create<ICompareFilterDefinitions>();
        definition_comparer
            .Setup(_ => _.DefinitionsAreEqual(filter_definition, filter_definition))
            .Returns(FilterValidationResult.Succeeded());

        cancellation_token = CancellationToken.None;
    };

    static protected FilterValidators filter_validators_with_services(Action<ContainerBuilder> callback)
    {
        var containerBuilder = new ContainerBuilder();
        callback(containerBuilder);
        return new FilterValidators(
            tenant_id,
            stream_processor_state_repository.Object,
            filter_definitions.Object,
            definition_comparer.Object,
            new AutofacServiceProvider(containerBuilder.Build()),
            Mock.Of<ILogger>());
    }
}