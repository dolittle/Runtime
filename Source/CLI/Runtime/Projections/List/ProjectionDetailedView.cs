// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Projections.List;

/// <summary>
/// Represents the detailed information for a Projection.
/// </summary>
/// <param name="Alias">The Projection alias.</param>
/// <param name="Projection">The Projection identifier.</param>
/// <param name="Scope">The Projection Scope.</param>
/// <param name="Status">The status of the Projection.</param>
/// <param name="LastSuccessfullyProcessed">The last time the Projection successfully processed an Event.</param>
/// <param name="CopyToMongoDB">Whether or not the Projection has copies stored in MongoDB.</param>
public record ProjectionDetailedView(
    string Alias,
    Guid Projection,
    string Scope,
    string Status,
    DateTimeOffset LastSuccessfullyProcessed,
    string CopyToMongoDB);
