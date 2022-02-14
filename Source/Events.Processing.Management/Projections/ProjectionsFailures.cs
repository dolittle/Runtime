// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Management.Projections;

/// <summary>
/// Holds the unique <see cref="FailureId">failure ids</see> for Projections.
/// </summary>
public static class ProjectionsFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'ProjectionNotRegistered' failure type.
    /// </summary>
    public static FailureId ProjectionNotRegistered => FailureId.Create("a4ffb5ba-b09f-48d5-8d70-43df5f819de4");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'ProjectionNotRegisteredForTenant' failure type.
    /// </summary>
    public static FailureId ProjectionNotRegisteredForTenant => FailureId.Create("d04f88e0-cd9c-4cf5-a095-914d6577f482");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'AlreadyReplayingProjection' failure type.
    /// </summary>
    public static FailureId AlreadyReplayingProjection => FailureId.Create("6afafd6f-1eed-4065-ba02-f24c89a548b4");
}
