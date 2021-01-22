// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Logging;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="FilterDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class FilterValidator : ICanValidateFilterFor<FilterDefinition>
    {
        readonly IValidateFilterByComparingStreams _byComparingStreams;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidator"/> class.
        /// </summary>
        /// <param name="byComparingStreams">The <see cref="IValidateFilterByComparingStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FilterValidator(
            IValidateFilterByComparingStreams byComparingStreams,
            ILogger logger)
        {
            _byComparingStreams = byComparingStreams;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterDefinition persistedDefinition, IFilterProcessor<FilterDefinition> filter, CancellationToken cancellationToken) =>
            _byComparingStreams.Validate(persistedDefinition, filter, cancellationToken);
    }
}
