// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Callbacks
{
    /// <summary>
    /// Represents a <see cref="ICanRegisterCallbacks"/>.
    /// </summary>
    public class ScheduledCallbackGroup
    {
        readonly List<DisposableCallback> _callbacks = new();
        readonly ILogger _logger;
        readonly TimeSpan _interval;
        readonly IMetricsCollector _metrics;
        DateTime _lastCalled;

        public ScheduledCallbackGroup(TimeSpan interval, ILogger logger, IMetricsCollector metrics)
        {
            _interval = interval;
            _logger = logger;
            _metrics = metrics;
            _lastCalled = DateTime.UtcNow - interval;
        }

        /// <summary>
        ///  Registers a callback.
        /// </summary>
        /// <param name="callback">The callback to register.</param>
        /// <returns>An <see cref="IDisposable"/>, which when disposed will deregister the callback.</returns>
        public IDisposable RegisterCallback(Action callback)
        {
            _metrics.IncrementTotalCallbacksRegistered();
            lock (_callbacks)
            {
                var scheduledCallback = new DisposableCallback(callback, UnregisterCallback);
                _callbacks.Add(scheduledCallback);
                return scheduledCallback;
            }
        }

        /// <summary>
        /// Calls all of the registered callbacks.
        /// </summary>
        public void CallRegisteredCallbacks()
        {
            lock (_callbacks)
            {
                _lastCalled = DateTime.UtcNow;
                foreach (var scheduledCallback in _callbacks)
                {
                    try
                    {
                        _metrics.IncrementTotalCallbacksCalled();
                        var stopwatch = Stopwatch.StartNew();
                        scheduledCallback.Callback();
                        stopwatch.Stop();
                        _metrics.AddToTotalCallbackTime(stopwatch.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        _metrics.IncrementTotalCallbacksFailed();
                        _logger.CallbackCallFailed(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the time until the next callback is scheduled
        /// </summary>
        public TimeSpan TimeToNextCall
            => _lastCalled + _interval - DateTime.UtcNow;

        void UnregisterCallback(DisposableCallback callback)
        {
            _metrics.IncrementTotalCallbacksUnregistered();
            lock (_callbacks)
            {
                _callbacks.Remove(callback);
            }
        }
    }
}
