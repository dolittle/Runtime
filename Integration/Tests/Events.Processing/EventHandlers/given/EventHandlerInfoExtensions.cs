// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Integration.Tests.Events.Processing.EventHandlers.given;

public static class EventHandlerInfoExtensions
{
    public static StreamProcessorId GetFilterStreamId(this EventHandlerInfo info)
        => new (info.Id.Scope, info.Id.EventHandler, StreamId.EventLog);
    public static StreamProcessorId GetEventProcessorStreamId(this EventHandlerInfo info)
        => new (info.Id.Scope, info.Id.EventHandler, info.Id.EventHandler.Value);
}