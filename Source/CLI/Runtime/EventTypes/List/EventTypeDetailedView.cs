// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes.List
{
    /// <summary>
    /// Represents the detailed information for an Event Type.
    /// </summary>
    /// <param name="EventTypeAlias">The Aggregate Root alias.</param>
    /// <param name="EventTypeId">The Aggregate Root Id.</param>
    public record EventTypeDetailedView(string EventTypeAlias, Guid EventTypeId);
}
