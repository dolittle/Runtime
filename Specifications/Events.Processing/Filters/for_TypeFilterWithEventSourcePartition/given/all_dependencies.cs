// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.given;

public class all_dependencies
{
    protected static Mock<IWriteEventsToStreams> writer;
    protected static ScopeId scope;

    Establish context = () =>
    {
        scope = Guid.NewGuid();
        writer = new Mock<IWriteEventsToStreams>();
    };
}