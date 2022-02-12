// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// The exception that gets thrown when validating occurred format and it's null or empty.
/// </summary>
public class OccurredFormatCannotBeNullOrEmpty : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OccurredFormatCannotBeNullOrEmpty"/> class.
    /// </summary>
    public OccurredFormatCannotBeNullOrEmpty()
        : base("Occurred format cannot be null or empty")
    {
    }
}
