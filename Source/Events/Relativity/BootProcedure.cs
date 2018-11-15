/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Applications.Configuration;
using Dolittle.Bootstrapping;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Resources.Configuration;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IEventHorizon"/>
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        int _canPerformCount = 10;
        readonly IEventHorizonsConfigurationManager _configuration;
        readonly IBarrier _barrier;
        readonly IResourceConfiguration _resourceConfiguration;
        readonly IBoundedContextLoader _boundedContextLoader;

        /// <summary>
        /// Initializes a new instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="configuration"><see cref="IEventHorizonsConfigurationManager">Configuration mananger</see></param>
        /// <param name="barrier"><see cref="IBarrier">Barrier</see> to penetrate towards an <see cref="IEventHorizon"/></param>
        /// <param name="resourceConfiguration"></param>
        /// <param name="boundedContextLoader"></param>
        public BootProcedure(
            IEventHorizonsConfigurationManager configuration,
            IBarrier barrier,
            IResourceConfiguration resourceConfiguration,
            IBoundedContextLoader boundedContextLoader)
        {
            _configuration = configuration;
            _barrier = barrier;
            _resourceConfiguration = resourceConfiguration;
            _boundedContextLoader = boundedContextLoader;
        }

        /// <inheritdoc/>
        public bool CanPerform() => (_resourceConfiguration.IsConfigured && _boundedContextLoader.ConfigurationIsLoaded) || _canPerformCount-- == 0;

        /// <inheritdoc/>
        public void Perform()
        {
            _configuration.Current.EventHorizons.ForEach(_ => 
                _barrier.Penetrate(
                    new EventHorizonKey(_.Application,_.BoundedContext),
                    _.Url,
                    _.Events)
                );
        }
    }
}
