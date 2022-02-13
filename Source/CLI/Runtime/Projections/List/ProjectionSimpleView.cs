// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.CLI.Runtime.Projections.List;

/// <summary>
/// Represents the base information for an Projection.
/// </summary>
/// <param name="Projection">The Projection identifier.</param>
/// <param name="Scope">The Projection Scope.</param>
/// <param name="Status">The status of the Projection.</param>
/// <param name="HasCopies">Whether or not the Projection has copies.</param>
public record ProjectionSimpleView(
    string Projection,
    string Scope,
    string Status,
    string HasCopies);
