// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents the concept of an artifact.
/// </summary>
/// <param name="Id">The artifact type identifier.</param>
/// <param name="Generation">The artifact generation.</param>
public record Artifact(
    Guid Id,
    uint Generation)
{
    /// <summary>
    /// Converts an <see cref="Artifact"/> to an <see cref="Artifacts.Artifact"/>.
    /// </summary>
    /// <param name="artifact">The artifact to convert.</param>
    /// <returns>The converted artifact.</returns>
    public static implicit operator Artifacts.Artifact(Artifact artifact)
        => new(artifact.Id, artifact.Generation);

    /// <summary>
    /// Converts an <see cref="Artifacts.Artifact"/> to an <see cref="Artifact"/>.
    /// </summary>
    /// <param name="artifact">The artifact to convert.</param>
    /// <returns>The converted artifact.</returns>
    public static implicit operator Artifact(Artifacts.Artifact artifact)
        => new(artifact.Id, artifact.Generation);
}
