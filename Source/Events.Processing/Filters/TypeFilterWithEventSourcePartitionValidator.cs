// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;


namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
/// </summary>
[Singleton, PerTenant]
public class TypeFilterWithEventSourcePartitionValidator : ICanValidateFilterFor<TypeFilterWithEventSourcePartitionDefinition>
{
    readonly IValidateFilterByComparingEventTypes _byComparingEventTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionValidator"/> class.
    /// </summary>
    /// <param name="byComparingEventTypes">The <see cref="IValidateFilterByComparingEventTypes" />.</param>
    public TypeFilterWithEventSourcePartitionValidator(
        IValidateFilterByComparingEventTypes byComparingEventTypes)
    {
        _byComparingEventTypes = byComparingEventTypes;
    }

    /// <inheritdoc/>
    public Task<FilterValidationResult> Validate(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken) =>
        _byComparingEventTypes.Validate(persistedDefinition, filter, lastUnprocessedEvent, cancellationToken);
}
