// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.given
{
    public class all_dependencies
    {
        protected static StreamId source_stream;
        protected static StreamId target_stream;
        protected static EventProcessorId event_processor_id;
        protected static ScopeId scope_id;
        protected static StreamProcessorId stream_processor_id;
        protected static Artifact filter_definition_event_type_one;
        protected static Artifact filter_definition_event_type_two;
        protected static Artifact filter_definition_event_type_three;
        protected static Artifact event_type_four;
        protected static TypeFilterWithEventSourcePartitionDefinition filter_definition;
        protected static IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter_processor;

        protected static Mock<ICanFetchEventTypesFromStream> types_fetcher;
        protected static Mock<IEventFetchers> events_fetchers;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_states;
        protected static ValidateFilterByComparingEventTypes validator;
        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
            var mocks = new MockRepository(MockBehavior.Strict);

            source_stream = StreamId.EventLog;
            target_stream = Guid.Parse("b057ebb0-23db-4180-9d77-e2c957dc20f2");
            event_processor_id = target_stream.Value;
            scope_id = Guid.Parse("4d55b68b-4925-4a59-8a1e-0e8228637014");
            stream_processor_id = new StreamProcessorId(scope_id, event_processor_id, source_stream);
            filter_definition_event_type_one = new Artifact("32d9142b-44fd-474c-bb1c-b302c4f9ea15", ArtifactGeneration.First);
            filter_definition_event_type_two = new Artifact("091160c2-3a0b-4d51-b81c-1a31a8f49037", ArtifactGeneration.First);
            filter_definition_event_type_three = new Artifact("738ac8bc-bc23-42c3-86df-89a0d7d221d3", ArtifactGeneration.First);
            event_type_four = new Artifact("3a8816c9-f6ee-487e-85af-4199fcfe210c", ArtifactGeneration.First);
            filter_definition = new TypeFilterWithEventSourcePartitionDefinition(
                source_stream,
                target_stream,
                new[] {
                    filter_definition_event_type_one.Id,
                    filter_definition_event_type_two.Id,
                    filter_definition_event_type_three.Id,
                },
                false);
            
            var filter_processor_mock = mocks.Create<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>>();
            filter_processor_mock.SetupGet(_ => _.Definition).Returns(filter_definition);
            filter_processor_mock.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            filter_processor_mock.SetupGet(_ => _.Scope).Returns(scope_id);
            filter_processor = filter_processor_mock.Object;

            types_fetcher = mocks.Create<ICanFetchEventTypesFromStream>();

            events_fetchers = mocks.Create<IEventFetchers>();
            events_fetchers
                .Setup(_ => _.GetTypeFetcherFor(scope_id, new EventLogStreamDefinition(), cancellation_token))
                .Returns(Task.FromResult(types_fetcher.Object));

            stream_processor_states = mocks.Create<IStreamProcessorStateRepository>();

            validator = new ValidateFilterByComparingEventTypes(events_fetchers.Object, stream_processor_states.Object);

            cancellation_token = CancellationToken.None;
        };
    }
}
