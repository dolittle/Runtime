// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    public class Filters : IFilters
    {
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly IFilterValidators _filterValidators;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Filters(
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            IFilterValidators filterValidators,
            ILogger<Filters> logger)
        {
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _filterValidators = filterValidators;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FilterRegistrationResult<TFilterDefinition>> Register<TFilterDefinition>(
            IFilterProcessor<TFilterDefinition> filterProcessor,
            StreamProcessorRegistrations streamProcessorRegistrations,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            try
            {
                var registrationResults = await _streamProcessorForAllTenants.Register(filterProcessor, filterProcessor.Definition.SourceStream, cancellationToken).ConfigureAwait(false);
                registrationResults.ForEach(streamProcessorRegistrations.Add);

                var filterValidationResult = await _filterValidators.Validate(filterProcessor, cancellationToken).ConfigureAwait(false);

                return filterValidationResult.Succeeded switch
                    {
                        true => new FilterRegistrationResult<TFilterDefinition>(filterProcessor),
                        _ => new FilterRegistrationResult<TFilterDefinition>($"Failed to register Filter: '{filterProcessor.Identifier}' on Stream: {filterProcessor.Definition.SourceStream}. {filterValidationResult.FailureReason}")
                    };
            }
            catch (Exception ex)
            {
                return new FilterRegistrationResult<TFilterDefinition>($"Failed to register Filter: '{filterProcessor.Identifier}' on Stream: {filterProcessor.Definition.SourceStream}. {ex.Message}");
            }
        }
    }
}