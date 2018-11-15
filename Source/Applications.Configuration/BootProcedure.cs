/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using System.Linq;
using Dolittle.Bootstrapping;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Resources.Configuration;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Performs the boot procedures for the application configuration
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IBoundedContextLoader _boundedContextLoader;
        readonly IExecutionContextManager _executionContextManager;
        readonly IResourceConfiguration _resourceConfiguration;
        /// <summary>
        /// Instantiates an instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="boundedContextLoader"></param>
        /// <param name="executionContextManager"></param>
        /// <param name="resourceConfiguration"></param>
        public BootProcedure(IBoundedContextLoader boundedContextLoader, IExecutionContextManager executionContextManager, IResourceConfiguration resourceConfiguration )
        {
            _boundedContextLoader = boundedContextLoader;
            _executionContextManager = executionContextManager;
            _resourceConfiguration = resourceConfiguration;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            var boundedContextConfig = _boundedContextLoader.Load();
            var environment = _executionContextManager.Current.Environment;
            
            ApplicationBindings.Application = boundedContextConfig.Application;
            ApplicationBindings.BoundedContext = boundedContextConfig.BoundedContext;

            _resourceConfiguration.ConfigureResourceTypes(boundedContextConfig.Resources.ToDictionary(kvp => kvp.Key, kvp => environment == Environment.Production? kvp.Value.Production : kvp.Value.Development));
        }
    }
}