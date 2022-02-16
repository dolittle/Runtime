// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;


namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="PublicFilterDefinition" />.
/// </summary>
[SingletonPerTenant]
public class PublicFilterValidator : ICanValidateFilterFor<PublicFilterDefinition>
{
    readonly IValidateFilterByComparingStreams _byComparingStreams;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicFilterValidator"/> class.
    /// </summary>
    /// <param name="byComparingStreams">The <see cref="IValidateFilterByComparingStreams" />.</param>
    public PublicFilterValidator(IValidateFilterByComparingStreams byComparingStreams)
    {
        _byComparingStreams = byComparingStreams;
    }

    /// <inheritdoc/>
    public Task<FilterValidationResult> Validate(PublicFilterDefinition persistedDefinition, IFilterProcessor<PublicFilterDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken) =>
        _byComparingStreams.Validate(persistedDefinition, filter, lastUnprocessedEvent, cancellationToken);
}
