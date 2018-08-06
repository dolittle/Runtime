/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizon"/>
    /// </summary>
    [Singleton]
    public class EventHorizon : IEventHorizon
    {
        /// <summary>
        /// 
        /// </summary>
        public EventHorizon()
        {
        }

        /// <inheritdoc/>
        public void PassThrough(CommittedEventStream committedEventStream)
        {
        }

        /// <inheritdoc/>
        public void Collapse(ISingularity singularity)
        {
            
        }

        /// <inheritdoc/>
        public void GravitateTowards(ISingularity singularity)
        {
        }
    }
}