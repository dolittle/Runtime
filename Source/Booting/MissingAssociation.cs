// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Exception that gets thrown when an association is missing.
/// </summary>
public class MissingAssociation : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingAssociation"/> class.
    /// </summary>
    /// <param name="key">Key for association that is missing.</param>
    public MissingAssociation(string key)
        : base($"Missing association with key '{key}'")
    {
    }
}