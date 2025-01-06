// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_EventProcessor.when_processing;

public class an_event : given.all_dependencies
{
    static EventProcessor processor;
    static IProcessingResult result;

    Establish context = () => processor = new EventProcessor(consent_id, subscription_id, external_events_committer.Object, metrics, logger);

    Because of = () => result = processor.Process(@event, partition, StreamPosition.Start, execution_context, default).GetAwaiter().GetResult();

    It should_commit_event = () => external_events_committer.Verify(_ => _.Commit(Moq.It.Is<CommittedEvents>(events => events.Count == 1 && events[0].Equals(@event)), consent_id, subscription_id.ScopeId), Moq.Times.Once);
    It should_return_succeeded_processing_result = () => result.Succeeded.ShouldBeTrue();
}