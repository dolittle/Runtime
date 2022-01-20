// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventProcessor;

public class when_creating_processor : given.all_dependencies
{
    static EventProcessor processor;

    Because of = () => processor = new EventProcessor(scope, event_processor_id, dispatcher.Object, Moq.Mock.Of<ILogger>());

    It should_have_the_correct_identifier = () => processor.Identifier.ShouldEqual(event_processor_id);
}