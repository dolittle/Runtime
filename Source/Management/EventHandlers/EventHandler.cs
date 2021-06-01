// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Management.EventHandlers
{
    public class EventHandler
    {
        public Guid Id { get; set; }
        public Guid SourceStream { get; set; }
        public int Position { get; set; }
        public bool Failed { get; set; }
    }
}