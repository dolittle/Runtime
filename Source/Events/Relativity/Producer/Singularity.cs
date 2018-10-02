/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="ISingularity"/>
    /// </summary>
    public class Singularity : ISingularity
    {
        readonly EventParticleSubscription _subscription;
        readonly IQuantumTunnel _tunnel;

        /// <summary>
        /// Initializes a new instance of <see cref="Singularity"/>
        /// </summary>
        /// <param name="application"><see cref="Application">Application</see> representing the singularity</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> representing the bounded context of the singularity</param>
        /// <param name="tunnel"><see cref="IQuantumTunnel"/> used to pass through to <see cref="Singularity"/></param>
        /// <param name="subscription"><see cref="EventParticleSubscription"/></param>
        public Singularity(
            Application application,
            BoundedContext boundedContext,
            IQuantumTunnel tunnel,
            EventParticleSubscription subscription)
        {
            _subscription = subscription;
            _tunnel = tunnel;
            Application = application;
            
            BoundedContext = boundedContext;
        }

        /// <inheritdoc/>
        public bool CanPassThrough(Processing.CommittedEventStreamWithContext committedEventStream)
        {
            return _subscription.Events.Any(_ => committedEventStream.EventStream.Events.Any(e => e.Metadata.Artifact == _));
        }

        /// <inheritdoc/>
        public bool PassThrough(Processing.CommittedEventStreamWithContext committedEventStream)
        {
            if( !CanPassThrough(committedEventStream)) return false;
            _tunnel.PassThrough(committedEventStream);

            return true;
        }

        /// <inheritdoc/>
        public Application Application { get; }

        /// <inheritdoc/>
        public BoundedContext BoundedContext { get; }
    }
}