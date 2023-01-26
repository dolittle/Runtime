// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents a <see cref="AbstractFilterProcessor{T}"/> that filters by known event types and can partition using an <see cref="EventSourceId"/>.
/// </summary>
public class TypeFilterWithEventSourcePartition : AbstractFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartition"/> class.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="definition">The<see cref="TypeFilterWithEventSourcePartitionDefinition"/>.</param>
    /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public TypeFilterWithEventSourcePartition(
        ScopeId scope,
        TypeFilterWithEventSourcePartitionDefinition definition,
        IWriteEventsToStreams eventsToStreamsWriter,
        ILogger<TypeFilterWithEventSourcePartition> logger) // TODO: Just regular logger? Probably some factories we can get rid of here
        : base(scope, definition, eventsToStreamsWriter, logger)
    {
    }

    /// <inheritdoc/>
    public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var included = Definition.Types.Contains(@event.Type.Id);
        var outPartitionId = PartitionId.None;
        if (Definition.Partitioned)
        {
            outPartitionId = @event.EventSource.Value;
        }

        return Task.FromResult<IFilterResult>(new SuccessfulFiltering(included, outPartitionId));
    }

    /// <inheritdoc/>
    public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
        => Filter(@event, partitionId, eventProcessorId, executionContext, cancellationToken);
}
