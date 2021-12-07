// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Defines a system that is capable of defining a named async resilience policy.
/// </summary>
public interface IDefineNamedAsyncPolicy : IDefineAsyncPolicy
{
    /// <summary>
    /// Gets the name of the policy.
    /// </summary>
    string Name { get; }
}