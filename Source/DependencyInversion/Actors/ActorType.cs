// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Defines the scoping types for a type registered in a DI container.
/// </summary>
public enum ActorType 
{
    None,
    Actor,
    Grain,
}
