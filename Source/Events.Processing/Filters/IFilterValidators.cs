// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows how to validate filters.
    /// </summary>
    public interface IFilterValidators
    {
        /// <summary>
        /// Validates a filter.
        /// </summary>
        /// <typeparam name="TDefinition">The subtype of <see cref="IFilterDefinition" />.</typeparam>
        /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="FilterValidationResult" />.</returns>
        Task<FilterValidationResult> Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition;
    }
}