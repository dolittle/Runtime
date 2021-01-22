// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.IO.Tenants
{
    /// <summary>
    /// Represents the <see cref="IConfigurationObject"/> for <see cref="Files"/>.
    /// </summary>
    [Singleton]
    public class FilesConfiguration
    {
        /// <summary>
        /// Gets or sets the root path of the file system to be used for tenants.
        /// </summary>
        public string RootPath { get; set; }
    }
}