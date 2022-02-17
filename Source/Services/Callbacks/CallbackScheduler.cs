// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Callbacks;

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
    readonly IMetricsCollector _metrics;
    readonly ConcurrentDictionary<TimeSpan, ScheduledCallbackGroup> _groups = new();
    readonly ManualResetEvent _waitForNewCallback = new(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackScheduler"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public CallbackScheduler(
        IHostApplicationLifetime hostApplicationLifetime,
        ILoggerFactory loggerFactory,
        IMetricsCollector metrics)
    {
        _hostApplicationStopping = hostApplicationLifetime.ApplicationStopping;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<CallbackScheduler>();
        _metrics = metrics;
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
            new ScheduledCallbackGroup(
                interval,
                _loggerFactory.CreateLogger<ScheduledCallbackGroup>(),
                _metrics));
        var disposableCallback = group.RegisterCallback(callback);
        _waitForNewCallback.Set();
        return disposableCallback;
    }

    void CallbackLoop()
    {
        while (!_hostApplicationStopping.IsCancellationRequested)
        {
            _waitForNewCallback.Reset();

            var timeToNextCall = TimeSpan.FromMilliseconds(int.MaxValue);
            try
            {
                foreach (var group in _groups.Values)
                {
                    var timeToNextGroupCall = group.TimeToNextCall;

                    if (timeToNextGroupCall < TimeSpan.Zero)
                    {
                        _metrics.IncrementTotalSchedulesMissed();
                        _metrics.AddToTotalSchedulesMissedTime(-timeToNextGroupCall);
                    }

                    if (timeToNextGroupCall <= _timeResolution)
                    {
                        group.CallRegisteredCallbacks();

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
                _metrics.IncrementTotalCallbackLoopsFailed();
                _logger.CallbackLoopFailed(ex);
            }

            if (timeToNextCall > _timeResolution)
            {
                _waitForNewCallback.WaitOne(timeToNextCall - _timeResolution);
            }
        }
    }
}
