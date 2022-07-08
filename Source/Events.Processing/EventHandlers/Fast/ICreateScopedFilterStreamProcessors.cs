// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

public interface ICreateScopedFilterStreamProcessors
{
    
    Task<ScopedFilterStreamProcessor> Create(
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        StreamProcessorId streamProcessorId,
        CancellationToken cancellationToken);
}
