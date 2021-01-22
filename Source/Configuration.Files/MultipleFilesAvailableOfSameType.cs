// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Configuration.Files
{
    /// <summary>
    /// Exception that gets thrown when there are multiple files available for same type of <see cref="IConfigurationObject"/>.
    /// </summary>
    public class MultipleFilesAvailableOfSameType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleFilesAvailableOfSameType"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/>.</param>
        /// <param name="paths">Collection of paths where files were found.</param>
        public MultipleFilesAvailableOfSameType(Type type, IEnumerable<string> paths)
            : base($"Configuration type '{type.AssemblyQualifiedName}' can be provided by multiple files: {string.Join(",", paths)} ")
        {
        }
    }
}