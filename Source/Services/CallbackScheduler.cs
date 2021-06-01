// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Services
{
    public class CallbackScheduler : ICanCallYouLater
    {

        public CallbackScheduler()
        {
        }

        public IDisposable TryRegisterCallback(Action callback, TimeSpan delay)
        {
            return new Timer((object state) => callback(), null, TimeSpan.Zero, delay);
        }
    }
}
