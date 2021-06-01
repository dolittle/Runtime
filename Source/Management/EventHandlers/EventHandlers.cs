// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Management.EventHandlers
{
    public class EventHandlers
    {
        public IEnumerable<EventHandler> AllForTenant(Guid tenantId)
        {
            return Array.Empty<EventHandler>();
        }
    }
}