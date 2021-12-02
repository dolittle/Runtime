// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Get;

/// <summary>
/// Represents the detailed information for an Aggregate Root Instance.
/// </summary>
/// <param name="Tenant">The Tenant Id.</param>
/// <param name="EventSource">The Event Source Id.</param>
/// <param name="AggregateRootVersion">The Aggregate Root Version.</param>
public record AggregateRootInstanceDetailedView(Guid Tenant, string EventSource, ulong AggregateRootVersion);