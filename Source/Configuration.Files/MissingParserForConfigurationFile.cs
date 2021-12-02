// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.Files;

/// <summary>
/// Exception that gets thrown when there is no parser capable of parsing the configuration file.
/// </summary>
public class MissingParserForConfigurationFile : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingParserForConfigurationFile"/> class.
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    public MissingParserForConfigurationFile(string filename)
        : base($"Missing parser for configuration file '{filename}'")
    {
    }
}