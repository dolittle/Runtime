// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_EventProcessor
{
    public class when_creating_processor : given.all_dependencies
    {
        static EventProcessor processor;

        Because of = () => processor = new EventProcessor(event_processor_id, call_dispatcher.Object, execution_context_manager.Object, Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => processor.Identifier.ShouldEqual(event_processor_id);
    }
}