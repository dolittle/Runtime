/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Coordination;

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Represents a <see cref="ICommandContextFactory"/>
    /// </summary>
    public class CommandContextFactory : ICommandContextFactory
    {
        readonly IUncommittedEventStreamCoordinator _uncommittedEventStreamCoordinator;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandContextFactory">CommandContextFactory</see>
        /// </summary>
        /// <param name="uncommittedEventStreamCoordinator">A <see cref="IUncommittedEventStreamCoordinator"/> to use for coordinator an <see cref="UncommittedEvents"/></param>
        /// <param name="executionContextManager">A <see cref="IExecutionContextManager"/> for getting execution context from</param>
        /// <param name="logger"><see cref="ILogger"/> to use for logging</param>
        public CommandContextFactory(
            IUncommittedEventStreamCoordinator uncommittedEventStreamCoordinator,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _uncommittedEventStreamCoordinator = uncommittedEventStreamCoordinator;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ICommandContext Build(CommandRequest command)
        {
            return new CommandContext(
                command,
                _executionContextManager.Current,
                _uncommittedEventStreamCoordinator,
                _logger
                );
        }
    }
}