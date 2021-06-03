// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
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
        static readonly TimeSpan _timeResolution = TimeSpan.FromMilliseconds(10);
        readonly CancellationToken _hostApplicationStopping;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        readonly ConcurrentDictionary<TimeSpan, ScheduledCallbackGroup> _groups = new();
        readonly ManualResetEvent _waitForNewCallback = new(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackScheduler"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public CallbackScheduler(
            IHostApplicationLifetime hostApplicationLifetime,
            ILoggerFactory loggerFactory)
        {
            _hostApplicationStopping = hostApplicationLifetime.ApplicationStopping;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<CallbackScheduler>();
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
            var group = _groups.GetOrAdd(interval,
                new ScheduledCallbackGroup(interval, _loggerFactory.CreateLogger<ScheduledCallbackGroup>()));
            var disposableCallback = group.RegisterCallback(callback);
            _waitForNewCallback.Set();
            return disposableCallback;
        }

        void CallbackLoop()
        {
            while (!_hostApplicationStopping.IsCancellationRequested)
            {
                _waitForNewCallback.Reset();

                var timeToNextCall = TimeSpan.MaxValue;
                try
                {
                    foreach (var group in _groups.Values)
                    {
                        var timeToNextGroupCall = group.TimeToNextCall;

                        if (timeToNextGroupCall < TimeSpan.Zero)
                        {
                            // AddToTotalSchedulesMissed
                            // AddToTotalSchedulesMissedTime(-timeToNextGroupCall)
                        }

                        if (timeToNextGroupCall <= _timeResolution)
                        {
                            try
                            {
                                group.CallRegisteredCallbacks();
                            }
                            catch (Exception ex)
                            {
                                // Incremendd
                                _logger.LogWarning(ex, "An error occured while calling registered callbacks");
                            }

                            timeToNextGroupCall = group.TimeToNextCall;
                        }

                        if (timeToNextGroupCall < timeToNextCall)
                        {
                            timeToNextCall = timeToNextGroupCall;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "An error occured while calling registered callbacks");
                }

                if (timeToNextCall > _timeResolution)
                {
                    _waitForNewCallback.WaitOne(timeToNextCall - _timeResolution);
                }
            }
        }
    }
}
