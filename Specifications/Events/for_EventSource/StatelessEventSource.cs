// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class StatelessEventSource : EventSource
    {
        public StatelessEventSource(Guid id)
            : base(id)
        {
        }
    }
}