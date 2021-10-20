// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Get
{
    
    /// <summary>
    /// Represents a simple view of an Event Handler state.
    /// </summary>
    /// <param name="Tenant">The Tenant.</param>
    /// <param name="Position">The stream position-</param>
    /// <param name="Status">The status.</param>
    public record EventHandlerSimpleView(Guid Tenant, ulong Position, string Status);
}