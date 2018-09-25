/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Bootstrapping;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IEventHorizon"/>
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IEventHorizonsConfigurationManager _configuration;
        readonly IBarrier _barrier;

        /// <summary>
        /// Initializes a new instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="configuration"><see cref="IEventHorizonsConfigurationManager">Configuration mananger</see></param>
        /// <param name="barrier"><see cref="IBarrier">Barrier</see> to penetrate towards an <see cref="IEventHorizon"/></param>
        public BootProcedure(
            IEventHorizonsConfigurationManager configuration,
            IBarrier barrier)
        {
            _configuration = configuration;
            _barrier = barrier;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

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
