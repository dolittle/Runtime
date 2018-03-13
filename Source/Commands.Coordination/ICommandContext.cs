/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Runtime.Commands;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Transactions;

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Defines a context for a <see cref="CommandRequest">command</see> passing through
    /// the system
    /// </summary>
    public interface ICommandContext : ITransaction
    {
        /// <summary>
        /// Gets the <see cref="TransactionCorrelationId"/> for the <see cref="ICommandContext"/>
        /// </summary>
        TransactionCorrelationId TransactionCorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="CommandRequest">command</see> the context is for
        /// </summary>
        CommandRequest Command { get; }

        /// <summary>
        /// Gets the <see cref="IExecutionContext"/> for the command
        /// </summary>
        IExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Register an <see cref="IEventSource">event source</see> for tracking
        /// </summary>
        /// <param name="eventSource"><see cref="IEventSource"/> being tracked</param>
        void RegisterForTracking(IEventSource eventSource);

        /// <summary>
        /// Get objects that are being tracked
        /// </summary>
        /// <returns>All tracked objects</returns>
        IEnumerable<IEventSource> GetObjectsBeingTracked();
    }
}