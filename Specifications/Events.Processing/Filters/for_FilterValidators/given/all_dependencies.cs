// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Versioning.Version;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.given
{
    public class all_dependencies
    {
        protected static Type filter_validator_type;
        protected static ScopeId scope_id;
        protected static StreamId filter_source_stream;
        protected static StreamId filter_target_stream;
        protected static StreamProcessorId stream_processor_id;
        protected static FilterDefinition filter_definition;
        protected static TypeFilterWithEventSourcePartitionDefinition different_filter_definition;
        protected static IFilterProcessor<FilterDefinition> filter_processor;
        protected static Mock<ICanValidateFilterFor<FilterDefinition>> filter_validator;
        protected static StreamProcessorState stream_processor_state;

        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_state_repository;
        protected static Mock<IFilterDefinitions> filter_definitions;
        protected static Mock<ICompareFilterDefinitions> definition_comparer;
        protected static Func<FilterValidators> filter_validators;
        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
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

            stream_processor_state = new StreamProcessorState(10, DateTimeOffset.Now);

            type_finder = mocks.Create<ITypeFinder>();
            type_finder
                .Setup(_ => _.FindMultiple<IFilterDefinition>())
                .Returns(new[] { typeof(FilterDefinition) });
            type_finder
                .Setup(_ => _.FindMultiple(typeof(ICanValidateFilterFor<FilterDefinition>)))
                .Returns(new[] { filter_validator_type });

            container = mocks.Create<IContainer>();
            container
                .Setup(_ => _.Get(filter_validator_type))
                .Returns(filter_validator.Object);

            stream_processor_state_repository = mocks.Create<IStreamProcessorStateRepository>();
            stream_processor_state_repository
                .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                .Returns(Task.FromResult(new Try<IStreamProcessorState>(true, stream_processor_state)));

            filter_definitions = mocks.Create<IFilterDefinitions>();
            filter_definitions
                .Setup(_ => _.TryGetFromStream(scope_id, filter_target_stream, cancellation_token))
                .Returns(Task.FromResult(new Try<IFilterDefinition>(true, filter_definition)));

            definition_comparer = mocks.Create<ICompareFilterDefinitions>();
            var validationResult = new FilterValidationResult();
            definition_comparer
                .Setup(_ => _.DefinitionsAreEqual(filter_definition, filter_definition, out validationResult))
                .Returns(true);

            var execution_context_manager = mocks.Create<IExecutionContextManager>();
            execution_context_manager
                .SetupGet(_ => _.Current)
                .Returns(new ExecutionContext(
                    Guid.Parse("c46727a1-fe79-4ac5-87d3-b70bd78c850d"),
                    Guid.Parse("776d700a-0b5a-415e-804f-b3fcff38fbef"),
                    new Version(1, 2, 3),
                    "environment",
                    Guid.Parse("e6a3777b-f7f1-4a0b-8944-2766c98f45c3"),
                    Claims.Empty,
                    CultureInfo.InvariantCulture));

            filter_validators = () => new FilterValidators(
                type_finder.Object,
                container.Object,
                () => stream_processor_state_repository.Object,
                () => filter_definitions.Object,
                execution_context_manager.Object,
                definition_comparer.Object,
                Mock.Of<ILogger>());

            cancellation_token = CancellationToken.None;
        };
    }
}