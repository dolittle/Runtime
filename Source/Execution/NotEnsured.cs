// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Exception that gets thrown when something is not <see cref="Ensure">ensured</see>.
/// </summary>
public class NotEnsured : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnsured"/> class.
    /// </summary>
    /// <param name="name">Name to associate that is not ensured.</param>
    public NotEnsured(string name)
        : base($"{name} cannot be null.")
    {
        Property = name;
    }

    /// <summary>
    /// Gets the property that failed ensure test.
    /// </summary>
    public string Property { get; }
}