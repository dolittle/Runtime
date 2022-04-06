// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Represents a range of <see cref="AggregateRootVersion" />.
/// </summary>
/// <param name="Start">The start of the range.</param>
/// <param name="End">The end of the range.</param>
public record AggregateRootVersionRange(AggregateRootVersion Start, AggregateRootVersion End);
