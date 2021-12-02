// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Defines a policy for a specific type.
/// </summary>
/// <typeparam name="T">Type the policy is for.</typeparam>
public interface IPolicyFor<T> : IPolicy
{
}