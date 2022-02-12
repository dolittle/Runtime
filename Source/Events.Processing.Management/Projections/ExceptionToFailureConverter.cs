// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

/// <summary>
/// Represents an implementation of <see cref="IExceptionToFailureConverter"/>.
/// </summary>
public class ExceptionToFailureConverter : IExceptionToFailureConverter
{
    /// <inheritdoc />
    public Failure ToFailure(Exception exception)
        => new(exception switch
            {
                ProjectionNotRegistered => ProjectionsFailures.ProjectionNotRegistered,
                StreamProcessorNotRegisteredForTenant => ProjectionsFailures.ProjectionNotRegisteredForTenant,
                AlreadySettingNewStreamProcessorPosition => ProjectionsFailures.AlreadyReplayingProjection,
                _ => FailureId.Other
            },
            exception.Message);
}
