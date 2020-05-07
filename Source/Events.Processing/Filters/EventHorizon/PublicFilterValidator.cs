// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="PublicFilterDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class PublicFilterValidator : ICanValidateFilterFor<PublicFilterDefinition>
    {
        readonly IValidateFilterByComparingStreams _byComparingStreams;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterValidator"/> class.
        /// </summary>
        /// <param name="byComparingStreams">The <see cref="IValidateFilterByComparingStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicFilterValidator(
            IValidateFilterByComparingStreams byComparingStreams,
            ILogger logger)
        {
            _byComparingStreams = byComparingStreams;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterDefinition persistedDefinition, IFilterProcessor<PublicFilterDefinition> filter, CancellationToken cancellationToken) =>
            _byComparingStreams.Validate(persistedDefinition, filter, cancellationToken);
    }
}