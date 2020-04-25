// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows how to register and start filters.
    /// </summary>
    public interface IFilters
    {
        /// <summary>
        /// Registers a filter.
        /// </summary>
        /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
        /// <param name="filterProcessor">The <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="streamProcessorRegistrations">The <see cref="StreamProcessorRegistrations" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A task.</returns>
        Task<FilterRegistrationResult<TFilterDefinition>> Register<TFilterDefinition>(
            IFilterProcessor<TFilterDefinition> filterProcessor,
            StreamProcessorRegistrations streamProcessorRegistrations,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition;
    }
}