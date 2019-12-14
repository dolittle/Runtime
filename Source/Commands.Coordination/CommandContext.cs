// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Coordination;

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Represents a <see cref="ICommandContext">ICommandContext</see>.
    /// </summary>
    public class CommandContext : ICommandContext
    {
        readonly IUncommittedEventStreamCoordinator _uncommittedEventStreamCoordinator;
        readonly List<IEventSource> _objectsTracked = new List<IEventSource>();

        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContext"/> class.
        /// </summary>
        /// <param name="command">The <see cref="CommandRequest">command</see> the context is for.</param>
        /// <param name="executionContext">The <see cref="ExecutionContext"/> for the command.</param>
        /// <param name="uncommittedEventStreamCoordinator">The <see cref="IUncommittedEventStreamCoordinator"/> to use for coordinating the committing of events.</param>
        /// <param name="logger"><see cref="ILogger"/> to use for logging.</param>
        public CommandContext(
            CommandRequest command,
            ExecutionContext executionContext,
            IUncommittedEventStreamCoordinator uncommittedEventStreamCoordinator,
            ILogger logger)
        {
            Command = command;
            ExecutionContext = executionContext;
            _uncommittedEventStreamCoordinator = uncommittedEventStreamCoordinator;
            _logger = logger;

            CorrelationId = CorrelationId.New();
        }

        /// <inheritdoc/>
        public CorrelationId CorrelationId { get; }

        /// <inheritdoc/>
        public CommandRequest Command { get; }

        /// <inheritdoc/>
        public ExecutionContext ExecutionContext { get; }

        /// <inheritdoc/>
        public void RegisterForTracking(IEventSource eventSource)
        {
            if (_objectsTracked.Contains(eventSource)) return;
            _objectsTracked.Add(eventSource);
        }

        /// <inheritdoc/>
        public IEnumerable<IEventSource> GetObjectsBeingTracked()
        {
            return _objectsTracked;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Commit();
        }

        /// <inheritdoc/>
        public void Commit()
        {
            _logger.Information("Commit transaction");
            var trackedObjects = GetObjectsBeingTracked();
            _logger.Trace($"Total number of objects tracked '{trackedObjects.Count()}");
            foreach (var trackedObject in trackedObjects)
            {
                _logger.Trace($"Committing events from {trackedObject.GetType().AssemblyQualifiedName}");
                var events = trackedObject.UncommittedEvents;
                if (events.HasEvents)
                {
                    _logger.Trace("Events present - send them to uncommitted eventstream coordinator");
                    _uncommittedEventStreamCoordinator.Commit(CorrelationId, events);
                    _logger.Trace("Commit object");
                    trackedObject.Commit();
                }
            }
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            // Todo : Should rollback any aggregated roots that are being tracked - this should really only be allowed to happen if we have not stored the events yet
            // once the events are stored, we can't roll back
        }
    }
}
