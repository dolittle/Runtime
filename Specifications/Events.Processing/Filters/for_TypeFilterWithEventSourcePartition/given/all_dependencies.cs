// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.given
{
    public class all_dependencies
    {
        protected static Mock<IWriteEventsToStreams> writer;

        Establish context = () => writer = new Mock<IWriteEventsToStreams>();
    }
}