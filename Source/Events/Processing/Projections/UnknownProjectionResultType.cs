// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Exception that gets thrown when a projection operation returned an <see cref="IProjectionResult"/> that is not known.
/// </summary>
public class UnknownProjectionResultType : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnknownProjectionResultType"/> class.
    /// </summary>
    /// <param name="result">The projection result that is not known.</param>
    public UnknownProjectionResultType(IProjectionResult result)
        : base($"IProjectionResult of type {result.GetType()} is unknown")
    {
    }
}