// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Defines a system that manages all policies.
/// </summary>
public interface IPolicies
{
    /// <summary>
    /// Gets the default <see cref="IPolicy"/>.
    /// </summary>
    /// <remarks>
    /// If nothing is <see cref="IDefineDefaultPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughPolicy"/> will be returned.
    /// </remarks>
    IPolicy Default { get; }

    /// <summary>
    /// Gets the default <see cref="IAsyncPolicy"/>.
    /// </summary>
    /// <remarks>
    /// If nothing is <see cref="IDefineDefaultAsyncPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughAsyncPolicy"/> will be returned.
    /// </remarks>
    IAsyncPolicy DefaultAsync { get; }

    /// <summary>
    /// Gets a named policy.
    /// </summary>
    /// <param name="name">Name of the policy.</param>
    /// <returns><see cref="IPolicy"/> to use.</returns>
    /// <remarks>
    /// If there is no policy with the given name, the system will return whatever is
    /// the default policy. If nothing is <see cref="IDefineDefaultPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughPolicy"/> will be returned.
    /// </remarks>
    INamedPolicy GetNamed(string name);

    /// <summary>
    /// Gets a named async policy.
    /// </summary>
    /// <param name="name">Name of the policy.</param>
    /// <returns><see cref="INamedAsyncPolicy"/> to use.</returns>
    /// <remarks>
    /// If there is no policy with the given name, the system will return whatever is
    /// the default policy. If nothing is <see cref="IDefineDefaultAsyncPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughAsyncPolicy"/> will be returned.
    /// </remarks>
    INamedAsyncPolicy GetAsyncNamed(string name);

    /// <summary>
    /// Get policy for a specific type.
    /// </summary>
    /// <typeparam name="T">Type to get policy for.</typeparam>
    /// <returns><see cref="IPolicy"/> to use.</returns>
    /// <remarks>
    /// If there is no policy with the given name, the system will return whatever is
    /// the default policy. If nothing is <see cref="IDefineDefaultPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughPolicy"/> will be returned.
    /// </remarks>
    IPolicyFor<T> GetFor<T>();

    /// <summary>
    /// Get async policy for a specific type.
    /// </summary>
    /// <typeparam name="T">Type to get policy for.</typeparam>
    /// <returns><see cref="IAsyncPolicyFor{T}"/> to use.</returns>
    /// <remarks>
    /// If there is no policy with the given name, the system will return whatever is
    /// the default policy. If nothing is <see cref="IDefineDefaultAsyncPolicy">defining</see>
    /// a default policy, a <see cref="PassThroughAsyncPolicy"/> will be returned.
    /// </remarks>
    IAsyncPolicyFor<T> GetAsyncFor<T>();
}