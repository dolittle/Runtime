// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Booting;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.ResourceTypes;
using ResourceType = Dolittle.Runtime.ResourceTypes.ResourceType;

namespace Dolittle.Runtime.Applications.Configuration
{
    /// <summary>
    /// Performs the boot procedures for the application configuration.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly IResourceConfiguration _resourceConfiguration;
        readonly BoundedContextConfiguration _boundedContextConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="boundedContextConfiguration"><see cref="BoundedContextConfiguration"/> to use.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> to use for <see cref="ExecutionContext"/>.</param>
        /// <param name="resourceConfiguration"><see cref="IResourceConfiguration">Configuration</see> of resources.</param>
        public BootProcedure(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            IResourceConfiguration resourceConfiguration)
        {
            _executionContextManager = executionContextManager;
            _resourceConfiguration = resourceConfiguration;
            _boundedContextConfiguration = boundedContextConfiguration;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            var environment = _executionContextManager.Current.Environment;

            var resourceTypes = new Dictionary<ResourceType, ResourceTypeImplementation>();
            foreach (var (resourceType, resourceImplementation) in _boundedContextConfiguration.Resources)
            {
                resourceTypes.Add(resourceType, environment == Environment.Production ? resourceImplementation.Production : resourceImplementation.Development);
            }
            _resourceConfiguration.ConfigureResourceTypes(resourceTypes);
        }
    }
}
