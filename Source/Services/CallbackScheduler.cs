// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents a <see cref="ICallbackScheduler"/>.
    /// </summary>
    [Singleton]
    public class CallbackScheduler : ICallbackScheduler
    {
        readonly ConcurrentDictionary<TimeSpan, CallbackRegister> _callbackDict = new();
        readonly IHostApplicationLifetime _hostApplicationLifetime;
        readonly ILogger _logger;
        readonly TimeSpan _defaultInterval = TimeSpan.FromSeconds(2);
        readonly TimeSpan _minimumInterval = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackScheduler"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public CallbackScheduler(
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
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
            var callbackRegister = _callbackDict.GetOrAdd(interval, new CallbackRegister());
            return callbackRegister.RegisterCallback(callback);
        }

        void CallbackLoop()
        {
            var threadInterval = _defaultInterval;
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                try
                {
                    threadInterval = GetSmallestInterval(threadInterval);

                    foreach (var item in _callbackDict)
                    {
                        var interval = item.Key;
                        var callbacks = item.Value;
                        if (ShouldBeCalled(callbacks.LastCalled, interval))
                        {
                            callbacks.CallRegisteredCallbacks();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "An error occured while calling registered callbacks");
                }

                Thread.Sleep(threadInterval);
            }
        }

        TimeSpan GetSmallestInterval(TimeSpan currentInterval)
        {
            var smallestInterval = _callbackDict.Keys.OrderBy(_ => _).FirstOrDefault();
            if (smallestInterval != default
                && smallestInterval < currentInterval
                && smallestInterval > _minimumInterval)
            {
                return smallestInterval;
            }
            return currentInterval;
        }

        bool ShouldBeCalled(DateTime lastCalled, TimeSpan interval) => lastCalled < DateTime.UtcNow - interval;
    }
}
