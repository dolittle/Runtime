// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

namespace Dolittle.Runtime.Services
{

    public class ScheduledCallback : IDisposable
    {
        bool _isRunning;

        public ScheduledCallback(Action callback, TimeSpan interval)
        {
            var thread = new Thread(() =>
            {
                do
                {
                    callback();
                    Thread.Sleep(interval);
                } while (_isRunning);
            })
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };
            _isRunning = true;
            thread.Start();
        }

        public void Dispose()
        {
            _isRunning = false;
            GC.SuppressFinalize(this);
        }
    }
}
