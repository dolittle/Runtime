// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

class GetCurrentProcessorState
{
    GetCurrentProcessorState()
    {
    }

    public static readonly GetCurrentProcessorState Instance = new();
};

record ReprocessEventsFrom(TenantId TenantId, ProcessingPosition ProcessingPosition);
