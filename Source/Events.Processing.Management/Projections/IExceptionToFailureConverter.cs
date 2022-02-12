// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

/// <summary>
/// Defines a system that can convert Projection Management exceptions to a failures.
/// </summary>
public interface IExceptionToFailureConverter
{
    /// <summary>
    /// Converts an <see cref="Exception"/> to a <see cref="Failure"/>.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to convert.</param>
    /// <returns>The converted <see cref="Failure"/>.</returns>
    Failure ToFailure(Exception exception);
}
