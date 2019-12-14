// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Represents a <see cref="ICommandContextManager">Command context manager</see>.
    /// </summary>
    public class CommandContextManager : ICommandContextManager
    {
        [ThreadStatic]
        static ICommandContext _currentContext;

        readonly ICommandContextFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContextManager"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="ICommandContextFactory"/> to use for building an <see cref="ICommandContext"/>.</param>
        public CommandContextManager(ICommandContextFactory factory)
        {
            _factory = factory;
        }

        /// <inheritdoc/>
        public bool HasCurrent
        {
            get { return CurrentContext != null; }
        }

        static ICommandContext CurrentContext
        {
            get { return _currentContext; }
            set { _currentContext = value; }
        }

        /// <summary>
        /// Reset context.
        /// </summary>
        public static void ResetContext()
        {
            CurrentContext = null;
        }

        /// <inheritdoc/>
        public ICommandContext GetCurrent()
        {
            if (!HasCurrent)
            {
                throw new InvalidOperationException("Command not established");
            }

            return CurrentContext;
        }

        /// <inheritdoc/>
        public ICommandContext EstablishForCommand(CommandRequest command)
        {
            if (!IsInContext(command))
            {
                var commandContext = _factory.Build(command);
                CurrentContext = commandContext;
            }

            return CurrentContext;
        }

        static bool IsInContext(CommandRequest command) => CurrentContext?.Command.Equals(command) == true;
    }
}