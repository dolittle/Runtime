// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;

namespace Dolittle.Runtime.Services
{
    public class CallbackRegister : ICanRegisterCallbacks
    {
        readonly ConcurrentDictionary<Guid, DisposableCallback> _callbacks = new();
        public DateTime LastCalled { get; private set; }

        public IDisposable RegisterCallback(Action callback)
        {
            var callbackId = Guid.NewGuid();
            var scheduledCallback = new DisposableCallback(callback, UnregisterCallback(callbackId));
            _callbacks.TryAdd(callbackId, scheduledCallback);
            return scheduledCallback;
        }

        public void CallRegisteredCallbacks()
        {
            foreach (var scheduledCallback in _callbacks.Values)
            {
                scheduledCallback.Callback();
            }
            LastCalled = DateTime.UtcNow;
        }

        Action UnregisterCallback(Guid callbackId)
        {
            return () =>
            {
                _callbacks.TryRemove(callbackId, out _);
            };
        }

    }
}
