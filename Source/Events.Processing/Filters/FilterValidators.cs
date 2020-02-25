// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterValidators" />.
    /// </summary>
    public class FilterValidators : IFilterValidators
    {
        readonly IContainer _container;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidators"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FilterValidators(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken = default)
            where TDefinition : IFilterDefinition
        {
            _logger.Debug($"Validating filter with filter definition type {typeof(TDefinition).FullName}");
            ICanValidateFilterFor<TDefinition> validator;
            try
            {
                validator = _container.Get<ICanValidateFilterFor<TDefinition>>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not resolve a filter validator for filter with definition type {typeof(TDefinition).FullName}");
                throw new CannotValidateFilterWithDefinitionType(typeof(TDefinition));
            }

            if (validator == null) throw new CannotValidateFilterWithDefinitionType(typeof(TDefinition));
            return validator.Validate(filter, cancellationToken);
        }
    }
}