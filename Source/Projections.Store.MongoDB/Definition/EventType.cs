// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition
{
    public class EventType
    {
        /// <summary>
        /// Gets or sets the type of the event type.
        /// </summary>
        public Guid Type { get; set; }

        /// <summary>
        /// Gets or sets the generation of the event type.
        /// </summary>
        public uint Generation { get; set; }
    }
}
