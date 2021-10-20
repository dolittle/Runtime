// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Events.Processing.EventHandlers;
namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when trying to get an Event Handler with the "Not Set" Event Handler alias.
    /// </summary>
    public class CannotGetEventHandlerWhenAliasIsNotSet : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotGetEventHandlerWhenAliasIsNotSet"/> class.
        /// </summary>
        public CannotGetEventHandlerWhenAliasIsNotSet()
            : base($"Cannot get a registered Event Handler with the '{EventHandlerAlias.NotSet.Value}' Event Handler alias ")
        { }
    }
}