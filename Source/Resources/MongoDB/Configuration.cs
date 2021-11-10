// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Represents the resource configuration for a MongoDB resource.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets the MongoDB host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not to use SSL.
        /// </summary>
        /// <remarks>This is not used for anything.</remarks>>
        public bool UseSSL { get; set; }
    }
}
