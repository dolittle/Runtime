// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Represents a base grain <see cref="Type"/> and the grain <see cref="IActor"/> <see cref="Type"/>. 
/// </summary>
/// <param name="Grain">The type of the grain.</param>
/// <param name="Actor">The type of the grain actor.</param>
/// <param name="IsPerTenant">Whether the grain is per tenant.</param>
public record GrainAndActor(Type Grain, Type Actor, string Kind, bool IsPerTenant)
{
}
