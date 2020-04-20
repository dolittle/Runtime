// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="PublicFilterDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class PublicFilterValidator : ICanValidateFilterFor<PublicFilterDefinition>
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterValidator"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicFilterValidator(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterProcessor<PublicFilterDefinition> filter, CancellationToken cancellationToken) =>
            Task.FromResult(new FilterValidationResult());
    }
}