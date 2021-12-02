// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams.Filters.for_TypeFilterWithEventSourcePartitionDefinition.given
{
    public static class artifacts
    {
        public static Artifact single() => new(Guid.NewGuid(), 1);
    }
}