// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Booting;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.ResourceTypes.Configuration;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IEventHorizon"/>.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly EventHorizonsConfiguration _eventHorizons;
        readonly IBarrier _barrier;
        readonly IResourceConfiguration _resourceConfiguration;
        int _canPerformCount = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="eventHorizons"><see cref="EventHorizonsConfiguration">Event horizons configuration</see>.</param>
        /// <param name="barrier"><see cref="IBarrier">Barrier</see> to penetrate towards an <see cref="IEventHorizon"/>.</param>
        /// <param name="resourceConfiguration"><see cref="IResourceConfiguration"/> for resources.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with <see cref="ExecutionContext"/>.</param>
        /// <param name="application">The running <see cref="Application"/>.</param>
        /// <param name="boundedContext">The running <see cref="BoundedContext"/>.</param>
        /// <param name="environment">The running environment.</param>
        public BootProcedure(
            EventHorizonsConfiguration eventHorizons,
            IBarrier barrier,
            IResourceConfiguration resourceConfiguration,
            IExecutionContextManager executionContextManager,
            Application application,
            BoundedContext boundedContext,
            Environment environment)
        {
            _eventHorizons = eventHorizons;
            _barrier = barrier;
            _resourceConfiguration = resourceConfiguration;

            executionContextManager.SetConstants(application, boundedContext, environment);
        }

        /// <inheritdoc/>
        public bool CanPerform() => _resourceConfiguration.IsConfigured || _canPerformCount-- == 0;

        /// <inheritdoc/>
        public void Perform()
        {
            _eventHorizons.ForEach(_ =>
                _barrier.Penetrate(
                    new EventHorizonKey(_.Application, _.BoundedContext),
                    _.Url,
                    _.Events));
        }
    }
}
