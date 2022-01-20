// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Callbacks;

/// <summary>
/// Defines a system that can register and call a set of callbacks.
/// </summary>
public interface ICanRegisterCallbacks
{
    /// <summary>
    ///  Registers a callback.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    /// <returns>An <see cref="IDisposable"/>, which when disposed will deregister the callback.</returns>
    IDisposable RegisterCallback(Action callback);

    /// <summary>
    /// Calls all of the registered callbacks.
    /// </summary>
    void CallRegisteredCallbacks();
}