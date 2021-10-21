// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when there is no registered Event Handler with the given Event Handler identifier.
    /// </summary>
    public class NoEventHandlerWithId : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerWithId"/> class.
        /// </summary>
        /// <param name="id">The Event Handler identifier.</param>
        public NoEventHandlerWithId(EventHandlerId id)
            : base($"There is no registered Event Handler with Id '{id.EventHandler.Value}' in Scope '{id.Scope.Value}'")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerWithId"/> class.
        /// </summary>
        /// <param name="alias">The Event Handler alias.</param>
        /// <param name="scope">The Event Handler Scope.</param>
        public NoEventHandlerWithId(EventHandlerAlias alias)
            : base($"There is no registered Event Handler with Alias '{alias.Value}'")
        { }
    }
}
