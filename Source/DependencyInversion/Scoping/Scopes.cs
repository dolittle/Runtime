// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Scoping;

/// <summary>
/// Defines the scoping types for a type registered in a DI container.
/// </summary>
public enum Scopes 
{
    /// <summary>
    /// Types registered in the global DI container will be shared across all execution contexts.
    /// </summary>
    Global,
    
    /// <summary>
    /// Types registered in the per-tenant DI containers will only be shared in execution contexts of the same tenant.
    /// </summary>
    PerTenant,
}
