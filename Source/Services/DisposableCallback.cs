// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{

    public class DisposableCallback : IDisposable
    {
        public DisposableCallback(Action callback, Action unregister)
        {
            Callback = callback;
            Unregister = unregister;
        }
        public Action Callback { get; internal set; }
        public Action Unregister { get; internal set; }

        public void Dispose()
        {
            Unregister();
            GC.SuppressFinalize(this);
        }
    }
}
