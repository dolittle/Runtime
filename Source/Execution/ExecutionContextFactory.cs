/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Globalization;
using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Tenancy;
using Dolittle.Security;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents a <see cref="IExecutionContextFactory"/>
    /// </summary>
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        ICanResolvePrincipal _principalResolver;
        IExecutionContextDetailsPopulator _detailsPopulator;
        Application _application;
        BoundedContext _boundedContext;
        IContainer _container;

        /// <summary>
        /// Initializes a new instance of <see cref="ExecutionContextFactory"/>
        /// </summary>
        /// <param name="principalResolver"><see cref="ICanResolvePrincipal"/> for resolving the identity</param>
        /// <param name="detailsPopulator">A <see cref="IExecutionContextDetailsPopulator"/> to use for populating any <see cref="IExecutionContext"/> being created</param>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="container">The <see cref="IContainer">IOC container</see> to resolve runtime dependencies</param>
        public ExecutionContextFactory(
            ICanResolvePrincipal principalResolver, 
            IExecutionContextDetailsPopulator detailsPopulator, 
            Application application, 
            BoundedContext boundedContext,
            IContainer container)
        {
            _principalResolver = principalResolver;
            _detailsPopulator = detailsPopulator;
            _application = application;
            _boundedContext = boundedContext;
            _container = container;
        }

        /// <inheritdoc/>
        public IExecutionContext Create()
        {
            var executionContext = new ExecutionContext(
                _principalResolver.Resolve(),
                CultureInfo.CurrentCulture,
                _detailsPopulator.Populate,
                _application,
                _boundedContext,
                _container.Get<ITenant>());

            return executionContext;
        }
    }
}
