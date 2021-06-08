// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Management.GraphQL.EventHandlers
{
    public class EventHandlerStatusForTenant
    {
        public Guid TenantId { get; set; }
        public int LastCommittedEventSequenceNumber { get; set; }
        public int FilterPosition { get; set; }
        public int EventProcessorPosition { get; set; }
    }
}