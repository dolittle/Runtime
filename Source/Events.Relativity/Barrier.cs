/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IBarrier"/>
    /// </summary>
    public class Barrier : IBarrier
    {
        readonly ILogger _logger;
        readonly Application _application;
        readonly BoundedContext _boundedContext;

        /// <summary>
        /// Initializes a new instance of <see cref="Barrier"/>
        /// </summary>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        public Barrier(Application application, BoundedContext boundedContext, ILogger logger)
        {
            _logger = logger;
            _application = application;
            _boundedContext = boundedContext;
        }

        /// <inheritdoc/>
        public void Penetrate(string url, IEnumerable<Artifact> events)
        {
            _logger.Information($"Penetrate barrier for quantum tunnel towards event horizon running at '{url}'");
            new Connection(_application, _boundedContext, url, events, _logger);
        }
    }

}