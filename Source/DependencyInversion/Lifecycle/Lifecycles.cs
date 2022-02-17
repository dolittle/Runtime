// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Lifecycle;

/// <summary>
/// Defines the lifecycle types for a type registered in a DI container.
/// </summary>
public enum Lifecycles
{
    /// <summary>
    /// Types registered as singleton will be instantiated once for the lifecycle of the application.
    /// </summary>
    Singleton,
    
    /// <summary>
    /// Types registered as scoped will be instantiated once for the lifecycle of a unit of work in the application.
    /// A unit of work is typically an incoming request.
    /// </summary>
    Scoped,
    
    /// <summary>
    /// Types registered as transient will be instantiated every time they are requested from the container.
    /// </summary>
    Transient,
}
