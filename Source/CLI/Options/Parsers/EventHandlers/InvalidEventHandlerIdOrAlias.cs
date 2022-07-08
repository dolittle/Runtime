// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.CLI.Runtime.EventHandlers;

namespace Dolittle.Runtime.CLI.Options.Parsers.EventHandlers;

/// <summary>
/// Exception that gets thrown when parsing of a <see cref="EventHandlerIdOrAlias"/> from a <see cref="string"/> fails.
/// </summary>
public class InvalidEventHandlerIdOrAlias : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEventHandlerIdOrAlias"/> class.
    /// </summary>
    /// <param name="idOrAlias">The address that failed to parse.</param>
    public InvalidEventHandlerIdOrAlias(string idOrAlias)
        : base($"The provided Event Handler Id or Alias '{idOrAlias}' is not valid. It needs to be either a string with the Event Handler Id Guid or Alias, or the Event Handler Id Guid or Alias and the Scope Id Guid separated by a ':'.")
    {
    }
}