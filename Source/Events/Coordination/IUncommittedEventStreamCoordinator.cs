/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Events.Coordination
{
    using Dolittle.Execution;

    /// <summary>
    /// Defines a coordinator for dealing with <see cref="UncommittedEventStream"/>
    /// </summary>
    public interface IUncommittedEventStreamCoordinator
    {
        /// <summary>
        /// Commit a <see cref="UncommittedEventStream"/>
        /// </summary>
        /// <param name="correlationId">
        /// The <see cref="CorrelationId"/> related to the request 
        /// the <see cref="UncommittedEventStream"/> was generated in
        /// </param>
        /// <param name="eventStream"><see cref="UncommittedEventStream"/> to commit</param>
        void Commit(CorrelationId correlationId, UncommittedEventStream eventStream);
    }
}
