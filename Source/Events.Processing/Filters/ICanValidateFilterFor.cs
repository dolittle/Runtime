// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that can validate a <see cref="IFilterProcessor{TDefinition}" /> for a specific processor.
    /// </summary>
    /// <typeparam name="TDefinition">The subtype of <see cref="IFilterDefinition" />.</typeparam>
    public interface ICanValidateFilterFor<TDefinition>
        where TDefinition : IFilterDefinition
    {
        /// <summary>
        /// Validates a filter of with definition <see typeparam="TDefinition" />.
        /// </summary>
        /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The async operation of validating a filter.</returns>
        Task Validate(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken);
    }
}