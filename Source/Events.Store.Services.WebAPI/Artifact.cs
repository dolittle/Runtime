// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

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
    public static implicit operator Dolittle.Artifacts.Contracts.Artifact(Artifact artifact)
        => new()
        {
            Generation = artifact.Generation,
            Id = artifact.Id.ToProtobuf()
        };

    /// <summary>
    /// Converts an <see cref="Artifacts.Artifact"/> to an <see cref="Artifact"/>.
    /// </summary>
    /// <param name="artifact">The artifact to convert.</param>
    /// <returns>The converted artifact.</returns>
    public static implicit operator Artifact(Dolittle.Artifacts.Contracts.Artifact artifact)
        => new(artifact.Id.ToGuid(), artifact.Generation);
}
