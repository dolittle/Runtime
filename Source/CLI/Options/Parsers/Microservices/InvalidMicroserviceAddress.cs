// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Microservices;

namespace Dolittle.Runtime.CLI.Options.Parsers.Microservices;

/// <summary>
/// Exception that gets thrown when parsing of a <see cref="MicroserviceAddress"/> from a <see cref="string"/> fails.
/// </summary>
public class InvalidMicroserviceAddress : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMicroserviceAddress"/> class.
    /// </summary>
    /// <param name="address">The address that failed to parse.</param>
    public InvalidMicroserviceAddress(string address)
        : base($"The provided address '{address}' is not a valid <host:port> combination.")
    {
    }
}