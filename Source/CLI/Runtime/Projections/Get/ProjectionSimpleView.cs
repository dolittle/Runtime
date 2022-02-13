// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Projections.Get;

/// <summary>
/// Represents a simple view of a Projection Stream Processor state.
/// </summary>
/// <param name="Tenant">The Tenant.</param>
/// <param name="Position">The stream position.</param>
/// <param name="Status">The status.</param>
public record ProjectionSimpleView(Guid Tenant, ulong Position, string Status);
