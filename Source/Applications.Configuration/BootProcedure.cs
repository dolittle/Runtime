/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using System.Linq;
using Dolittle.Booting;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.ResourceTypes.Configuration;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Performs the boot procedures for the application configuration
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly IResourceConfiguration _resourceConfiguration;
        readonly BoundedContextConfiguration _boundedContextConfiguration;

        /// <summary>
        /// Instantiates an instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="boundedContextConfiguration"><see cref="BoundedContextConfiguration"/> to use</param>
        /// <param name="executionContextManager"></param>
        /// <param name="resourceConfiguration"></param>
        public BootProcedure(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            IResourceConfiguration resourceConfiguration )
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

            _resourceConfiguration.ConfigureResourceTypes(
                _boundedContextConfiguration.Resources.ToDictionary(
                    kvp => kvp.Key, 
                    kvp => environment == Environment.Production? kvp.Value.Production : kvp.Value.Development));
        }
    }
}