// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.ApplicationModel;

/// <summary>
/// Represents the concept of a tenant.
/// </summary>
public record TenantId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Gets the tenant used when outside the scope of a tenant, typically the system.
    /// </summary>
    public static readonly TenantId System = Guid.Parse("08831584-e016-42f6-bc5e-c4f098fed42b");

    /// <summary>
    /// Gets the tenant used for development.
    /// </summary>
    public static readonly TenantId Development = Guid.Parse("445f8ea8-1a6f-40d7-b2fc-796dba92dc44");

    /// <summary>
    /// Implicitly convert from <see cref="Guid"/> to <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenantId"><see cref="Guid"/> representation of a tenant identifier.</param>
    public static implicit operator TenantId(Guid tenantId) => new(tenantId);

    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenantId"><see cref="string"/> representation of a tenant identifier.</param>
    public static implicit operator TenantId(string tenantId) => new(Guid.Parse(tenantId));
}
