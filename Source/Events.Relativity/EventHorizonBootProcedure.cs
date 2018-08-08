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
    public class EventHorizonBootProcedure : ICanPerformBootProcedure
    {
        readonly IEventHorizonsConfigurationManager _configuration;
        readonly IBarrier _barrier;

        /// <summary>
        /// 
        /// </summary>
        public EventHorizonBootProcedure(IEventHorizonsConfigurationManager configuration, IBarrier barrier, IEventHorizon eventHorizon)
        {
            _configuration = configuration;
            _barrier = barrier;
        }

        /// <inheritdoc/>
        public void Perform()
        {
            _configuration.Current.EventHorizons.ForEach(eventHorizon => _barrier.Penetrate(eventHorizon.Url, eventHorizon.Events));
        }
    }
}
