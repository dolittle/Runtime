// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Management.GraphQL.EventHandlers
{
    public class EventHandlers
    {
        public IEnumerable<EventHandler> AllForTenant(Guid tenantId)
        {
            return new EventHandler[] {
                new () { Id = Guid.Parse("0571cc9f-1c14-4e05-81cd-18a7557e56c0"), SourceStream = Guid.Empty, Position=0, Failed = false },
                new () { Id = Guid.Parse("c509ff72-6214-46da-b0c6-7c0ee8f90674"), SourceStream = Guid.Empty, Position=0, Failed = false },
                new () { Id = Guid.Parse("4822545c-add5-4d10-8b57-7993f559e923"), SourceStream = Guid.Empty, Position=0, Failed = false }
            };
        }
    }
}