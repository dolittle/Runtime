/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications;
using Dolittle.Applications.Configuration;
using Dolittle.Booting;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.ResourceTypes.Configuration;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IEventHorizon"/>
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        int _canPerformCount = 10;
        readonly EventHorizonsConfiguration _configuration;
        readonly IBarrier _barrier;
        readonly IResourceConfiguration _resourceConfiguration;

        /// <summary>
        /// Initializes a new instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="configuration"><see cref="EventHorizonsConfiguration">Configuration</see></param>
        /// <param name="barrier"><see cref="IBarrier">Barrier</see> to penetrate towards an <see cref="IEventHorizon"/></param>
        /// <param name="resourceConfiguration"></param>
        /// <param name="executionContextManager"></param>
        /// <param name="application"></param>
        /// <param name="boundedContext"></param>
        /// <param name="environment">The running environment</param>
        public BootProcedure(
            EventHorizonsConfiguration configuration,
            IBarrier barrier,
            IResourceConfiguration resourceConfiguration,
            IExecutionContextManager executionContextManager,
            Application application,
            BoundedContext boundedContext,
            Environment environment)
        {
            _configuration = configuration;
            _barrier = barrier;
            _resourceConfiguration = resourceConfiguration;

            executionContextManager.SetConstants(application, boundedContext, environment);
        }

        /// <inheritdoc/>
        public bool CanPerform() => _resourceConfiguration.IsConfigured || _canPerformCount-- == 0;

        /// <inheritdoc/>
        public void Perform()
        {
            _configuration.EventHorizons.ForEach(_ => 
                _barrier.Penetrate(
                    new EventHorizonKey(_.Application,_.BoundedContext),
                    _.Url,
                    _.Events)
                );
        }
    }
}
