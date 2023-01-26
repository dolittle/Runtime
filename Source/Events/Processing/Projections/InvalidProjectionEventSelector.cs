// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Exception that gets thrown when an invalid projection event selector case was received.
/// </summary>
public class InvalidProjectionEventSelector : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidProjectionEventSelector" /> class.
    /// </summary>
    /// <param name="selectorCase">The projection event selector case.</param>
    public InvalidProjectionEventSelector(ProjectionEventSelector.SelectorOneofCase selectorCase)
        : base($"Received invalid {nameof(ProjectionEventSelector)} {selectorCase} ")
    {
    }
}