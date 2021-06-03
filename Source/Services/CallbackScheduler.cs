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
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    foreach (var item in _callbackDict)
                    {
                        var interval = item.Key;
                        var callbacks = item.Value;
                        if (ShouldBeCalled(callbacks.LastCalled, now, interval))
                        {
                            callbacks.CallRegisteredCallbacks();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "An error occured while calling registered callbacks");
                }

                Thread.Sleep(GetNextExpiringInterval());
            }
        }

        TimeSpan GetNextExpiringInterval()
        {
            var nextExpiringIntervall = _callbackDict
               .Select(kvp => kvp.Value.LastCalled + kvp.Key)
               .OrderBy(projectedTime => projectedTime)
               .FirstOrDefault();
            if (nextExpiringIntervall == default)
            {
                return _defaultInterval;
            }
            var nextCall = DateTime.UtcNow - nextExpiringIntervall;
            var minimumInterval = TimeSpan.FromMilliseconds(10);
            return nextCall > minimumInterval ? nextCall : minimumInterval;
        }

        bool ShouldBeCalled(DateTime lastCalled, DateTime now, TimeSpan interval) => lastCalled < now - interval;
    }
}
