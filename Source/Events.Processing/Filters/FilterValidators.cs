// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterValidators" />.
    /// </summary>
    public class FilterValidators : IFilterValidators
    {
        readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidators"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer" />.</param>
        public FilterValidators(IContainer container)
        {
            _container = container;
        }

        /// <inheritdoc/>
        public Task Validate<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken = default)
            where TDefinition : IFilterDefinition
        {
            ICanValidateFilterFor<TDefinition> validator;
            try
            {
                validator = _container.Get<ICanValidateFilterFor<TDefinition>>();
            }
            catch (Exception)
            {
                throw new CannotValidateFilterWithDefinitionType(typeof(TDefinition));
            }

            if (validator == null) throw new CannotValidateFilterWithDefinitionType(typeof(TDefinition));
            return validator.Validate(filter, cancellationToken);
        }
    }
}