// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Commands.Coordination
{
    /// <summary>
    /// Creates <see cref="ICommandContext"/> for <see cref="CommandRequest"/>.
    /// </summary>
    public interface ICommandContextFactory
    {
        /// <summary>
        /// Creates an <see cref="ICommandContext"/> for a specific <see cref="CommandRequest" />.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest" /> to create a context for.</param>
        /// <returns>An <see cref="ICommandContext"/> for the specified <see cref="CommandRequest"/>.</returns>
        ICommandContext Build(CommandRequest command);
    }
}