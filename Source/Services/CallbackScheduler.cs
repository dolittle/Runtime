// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents a <see cref="ICanScheduleCallbacks"/>.
    /// </summary>
    [Singleton]
    public class CallbackScheduler : ICanScheduleCallbacks
    {
        readonly ConcurrentDictionary<TimeSpan, CallbackRegister> _callbackDict;
        readonly IHostApplicationLifetime _hostApplicationLifetime;
        readonly CancellationToken _token;

        public CallbackScheduler(CancellationToken token)
        {
            _callbackDict = new();
            _token = token;
            _hostApplicationLifetime = default;
            var thread = new Thread(CallbackLoop)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };
            thread.Start();
        }

        public CallbackScheduler(IHostApplicationLifetime hostApplicationLifetime)
        {
            _callbackDict = new();
            _hostApplicationLifetime = hostApplicationLifetime;
            var thread = new Thread(CallbackLoop)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };
            thread.Start();
        }

        /// <inheritdoc/>
        public IDisposable ScheduleCallback(Action callback, TimeSpan interval)
        {
            if (_callbackDict.TryGetValue(interval, out var callbacks))
            {
                return callbacks.RegisterCallback(callback);
            }
            else
            {
                var scheduledCallbacks = new CallbackRegister();
                var scheduledCallback = scheduledCallbacks.RegisterCallback(callback);
                if (!_callbackDict.TryAdd(interval, scheduledCallbacks))
                {
                    //Something went very wrong, maybe jsut retry as the problems should fix themselvs
                    return ScheduleCallback(callback, interval);
                }
                return scheduledCallback;
            }
        }

        void CallbackLoop()
        {
            var threadInterval = TimeSpan.FromMilliseconds(250);
            // while (!_token.IsCancellationRequested)
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                var smallestInterval = _callbackDict.Keys.OrderBy(_ => _).FirstOrDefault();
                if (smallestInterval != default && smallestInterval < threadInterval)
                {
                    threadInterval = smallestInterval;
                }

                foreach (var item in _callbackDict)
                {
                    var interval = item.Key;
                    var callbacks = item.Value;
                    if (callbacks.LastCalled < DateTime.UtcNow - interval)
                    {
                        callbacks.CallRegisteredCallbacks();
                    }
                }

                Thread.Sleep(threadInterval);
            }
            Console.WriteLine("DoneWith CallbackLoop");
        }
    }
}
