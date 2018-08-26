/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizon"/>
    /// </summary>
    [Singleton]
    public class EventHorizon : IEventHorizon
    {
        readonly List<ISingularity> _singularities = new List<ISingularity>();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventHorizon"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public EventHorizon(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void PassThrough(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream)
        {
            lock(_singularities)
            {
                _logger.Information($"Passing committed events through {_singularities.Count} singularities");
                _singularities
                    .Where(_ => _.CanPassThrough(committedEventStream)).AsParallel()
                    .ForEach(_ =>
                    {
                        _.PassThrough(committedEventStream);
                    });
            }
        }

        /// <inheritdoc/>
        public void Collapse(ISingularity singularity)
        {
            lock(_singularities)
            {
                _logger.Information($"Quantum tunnel collapsed for singularity identified with bounded context '{singularity.BoundedContext}' in application '{singularity.Application}'");
                _singularities.Remove(singularity);
            }
        }

        /// <inheritdoc/>
        public void GravitateTowards(ISingularity singularity)
        {
            lock(_singularities)
            {
                _logger.Information($"Gravitate events in the event horizon towards singularity identified with bounded context '{singularity.BoundedContext}' in application '{singularity.Application}'");
                _singularities.Add(singularity);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ISingularity> Singularities
        {
            get
            {
                lock(_singularities) return _singularities;
            }
        }
    }
}