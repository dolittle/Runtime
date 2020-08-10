// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class TypeFilterWithEventSourcePartitionValidator : ICanValidateFilterFor<TypeFilterWithEventSourcePartitionDefinition>
    {
        readonly IValidateFilterByComparingStreams _byComparingStreams;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionValidator"/> class.
        /// </summary>
        /// <param name="byComparingStreams">The <see cref="IValidateFilterByComparingStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public TypeFilterWithEventSourcePartitionValidator(
            IValidateFilterByComparingStreams byComparingStreams,
            ILogger logger)
        {
            _byComparingStreams = byComparingStreams;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken) =>
            _byComparingStreams.Validate(persistedDefinition, filter, cancellationToken);
    }
}