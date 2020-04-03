// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterValidators" />.
    /// </summary>
    public class FilterValidators : IFilterValidators
    {
        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidators"/> class.
        /// </summary>
        /// <param name="typeFinder">The <see cref="ITypeFinder" />.</param>
        /// <param name="container">The <see cref="IContainer" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FilterValidators(ITypeFinder typeFinder, IContainer container, ILogger logger)
        {
            _typeFinder = typeFinder;
            _container = container;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken = default)
            where TDefinition : IFilterDefinition
        {
            _logger.Trace($"Finding validator for filter '{filter.Definition.TargetStream}'");

            if (TryGetValidatorFor<TDefinition>(out var validator))
            {
                _logger.Trace($"Validating filter '{filter.Definition.TargetStream}'");
                return validator.Validate(filter, cancellationToken);
            }

            return Task.FromResult(new FilterValidationResult());
        }

        bool TryGetValidatorFor<TDefinition>(out ICanValidateFilterFor<TDefinition> validator)
            where TDefinition : IFilterDefinition
        {
            var implementations = _typeFinder.FindMultiple(typeof(ICanValidateFilterFor<TDefinition>));
            if (implementations.Any())
            {
                if (implementations.Count() > 1)
                {
                    _logger.Warning($"There are multiple validators that can validate filter defintion of type {typeof(TDefinition).FullName}:\n{string.Join("\n", implementations.Select(_ => _.FullName))}\nUsing the first validator.");
                }

                validator = _container.Get(implementations.First()) as ICanValidateFilterFor<TDefinition>;
                return true;
            }

            validator = null;
            return false;
        }
    }
}