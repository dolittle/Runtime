// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.Files
{
    /// <summary>
    /// Represents a system that knows about all <see cref="ICanParseConfigurationFile">configuration file parsers</see>.
    /// </summary>
    public interface IConfigurationFileParsers
    {
        /// <summary>
        /// Parse a file for a given <see cref="IConfigurationObject">configuration object type</see>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/>.</param>
        /// <param name="filename">The filename of the configuration.</param>
        /// <param name="content">Content to use.</param>
        /// <returns>Instance of the given <see cref="IConfigurationObject"/>.</returns>
        object Parse(Type type, string filename, string content);
    }
}