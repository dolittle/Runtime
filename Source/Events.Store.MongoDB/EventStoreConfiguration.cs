// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Represents a resource configuration for a MongoDB Read model implementation.
    /// </summary>
    public class EventStoreConfiguration
    {
        /// <summary>
        /// Gets or sets the MongoDB servers.
        /// </summary>
        public IEnumerable<string> Servers { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Database { get; set; }
    }
}
