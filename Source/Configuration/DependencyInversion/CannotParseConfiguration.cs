// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Exception that gets thrown when a tenant-specific configuration is attempted resolved from the root container.
/// </summary>
public class CannotParseConfiguration : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotParseConfiguration"/> class.
    /// </summary>
    public CannotParseConfiguration(Exception error, Type configuration, string section)
        : base($"Cannot parse {configuration} from section {section}", error)
    {
    }
}
