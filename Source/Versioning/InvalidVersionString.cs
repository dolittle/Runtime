// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Versioning;

/// <summary>
/// Exception that gets thrown when a version string is invalid.
/// </summary>
public class InvalidVersionString : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidVersionString"/> class.
    /// </summary>
    /// <param name="versionAsString">The string that holds the invalid <see cref="Version"/>.</param>
    public InvalidVersionString(string versionAsString)
        : base($"The '{versionAsString}' is not a valid version string")
    {
    }
}