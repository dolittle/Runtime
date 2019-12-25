// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Manages command contexts.
    /// </summary>
    public interface ICommandContextManager
    {
        /// <summary>
        /// Gets a value indicating whether or not we have a current command context.
        /// </summary>
        bool HasCurrent { get; }

        /// <summary>
        /// Gets the current <see cref="ICommandContext">command context</see>, if any.
        /// </summary>
        /// <returns>
        /// The current <see cref="ICommandContext">command context</see>.
        /// If there is no current context, it will throw an InvalidOperationException.
        /// </returns>
        ICommandContext GetCurrent();

        /// <summary>
        /// Establish a <see cref="ICommandContext">command context</see> for a specific <see cref="CommandRequest">command</see>.
        /// This will be the current command context, unless something else establishes a new context.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest">Command</see> to establish for.</param>
        /// <returns>Established context.</returns>
        /// <remarks>
        /// The contexts are not stacked. So establishing two contexts after one another does not give you a chance to
        /// go back up the "stack".
        /// </remarks>
        ICommandContext EstablishForCommand(CommandRequest command);
    }
}
