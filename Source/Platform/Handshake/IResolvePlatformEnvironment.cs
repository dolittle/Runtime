// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Defines a system that can resolve the <see cref="PlatformEnvironment"/>.
/// </summary>
public interface IResolvePlatformEnvironment
{
    /// <summary>
    /// Resolves the <see cref="PlatformEnvironment"/>.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, </returns>
    Task<PlatformEnvironment> Resolve();
}
