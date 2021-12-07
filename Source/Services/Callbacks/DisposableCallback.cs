// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Callbacks;

/// <summary>
/// Represents a callback with a callback on it's Dispose() method.
/// </summary>
public class DisposableCallback : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableCallback"/> class.
    /// </summary>
    /// <param name="callback">The <see cref="Action"/>.</param>
    /// <param name="unregister">The <see cref="Action"/> to call when this instance is disposed.</param>
    public DisposableCallback(Action callback, Action<DisposableCallback> unregister)
    {
        Callback = callback;
        Unregister = unregister;
    }
    /// <summary>
    /// Gets the callback.
    /// </summary>
    public Action Callback { get; }
    /// <summary>
    /// Gets the callback that's called when disposed of.
    /// </summary>
    public Action<DisposableCallback> Unregister { get; }

    public void Dispose()
    {
        Unregister(this);
        GC.SuppressFinalize(this);
    }
}