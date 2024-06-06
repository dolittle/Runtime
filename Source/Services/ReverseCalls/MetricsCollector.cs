// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.ReverseCalls;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Gauge _currentPendingStreamWrites;
    readonly Counter _totalStreamWrites;
    readonly Counter _totalStreamWriteBytes;
    readonly Counter _totalStreamWriteWaitTime;
    readonly Counter _totalStreamWriteTime;
    readonly Counter _totalStreamReads;
    readonly Counter _totalStreamReadBytes;
    readonly Counter _totalPingsSent;
    readonly Counter _totalPongsReceived;
    readonly Counter _totalKeepaliveResets;
    readonly Counter _totalKeepaliveTimeouts;
    readonly Counter _totalFirstMessageWaitTime;

    readonly UpDownCounter<long> _currentPendingStreamWritesOtel;
    readonly Counter<long> _totalStreamWritesOtel;
    readonly Counter<long> _totalStreamWriteBytesOtel;
    readonly Counter<double> _totalStreamWriteWaitTimeOtel;
    readonly Counter<double> _totalStreamWriteTimeOtel;
    readonly Counter<long> _totalStreamReadsOtel;
    readonly Counter<long> _totalStreamReadBytesOtel;
    readonly Counter<long> _totalPingsSentOtel;
    readonly Counter<long> _totalPongsReceivedOtel;
    readonly Counter<long> _totalKeepaliveResetsOtel;
    readonly Counter<long> _totalKeepaliveTimeoutsOtel;
    readonly Counter<double> _totalFirstMessageWaitTimeOtel;
    
    public MetricsCollector(IMetricFactory metricFactory)
    {
        _currentPendingStreamWrites = metricFactory.CreateGauge(
            "dolittle_system_runtime_services_reversecalls_pending_writes",
            "ReverseCall current pending stream writes waiting");

        _totalStreamWrites = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_writes_total",
            "ReverseCall total number of writes to streams");

        _totalStreamWriteBytes = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_write_bytes_total",
            "ReverseCall total number of bytes written to streams");

        _totalStreamWriteWaitTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_write_wait_seconds_total",
            "ReverseCall total time spent waiting to write to streams");

        _totalStreamWriteTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_write_seconds_total",
            "ReverseCall total time spent writing to streams");

        _totalStreamReads = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_reads_total",
            "ReverseCall total number of reads from streams");

        _totalStreamReadBytes = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_stream_read_bytes_total",
            "ReverseCall total number of bytes read from streams");

        _totalPingsSent = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_pings_sent_total",
            "ReverseCall total number of pings sent");

        _totalPongsReceived = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_pongs_received_total",
            "ReverseCall total number of pongs received");

        _totalKeepaliveResets = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_keepalive_resets_total",
            "ReverseCall total number of times ping keepalive tokens have been reset");

        _totalKeepaliveTimeouts = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_keepalive_timeouts_total",
            "ReverseCall total number of times ping keepalive tokens have timed out");

        _totalFirstMessageWaitTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reversecalls_first_message_wait_seconds_total",
            "ReverseCall total time spent waiting for first message");
        
        _currentPendingStreamWritesOtel = RuntimeMetrics.Meter.CreateUpDownCounter<long>(
            "dolittle_system_runtime_services_reversecalls_pending_writes",
            "ReverseCall current pending stream writes waiting");

        _totalStreamWritesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_stream_writes_total",
            "ReverseCall total number of writes to streams");

        
        // Opentelemetry
        _totalStreamWriteBytesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_stream_write_bytes_total",
            "bytes",
            "ReverseCall total number of bytes written to streams");

        _totalStreamWriteWaitTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_reversecalls_stream_write_wait_seconds_total",
            "seconds",
            "ReverseCall total time spent waiting to write to streams");

        _totalStreamWriteTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_reversecalls_stream_write_seconds_total",
            "seconds",
            "ReverseCall total time spent writing to streams");

        _totalStreamReadsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_stream_reads_total",
            "count",
            "ReverseCall total number of reads from streams");

        _totalStreamReadBytesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_stream_read_bytes_total",
            "bytes",
            "ReverseCall total number of bytes read from streams");

        _totalPingsSentOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_pings_sent_total",
            "ReverseCall total number of pings sent");

        _totalPongsReceivedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_pongs_received_total",
            "count",
            "ReverseCall total number of pongs received");

        _totalKeepaliveResetsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_keepalive_resets_total",
            "count",
            "ReverseCall total number of times ping keepalive tokens have been reset");

        _totalKeepaliveTimeoutsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reversecalls_keepalive_timeouts_total",
            "count",
            "ReverseCall total number of times ping keepalive tokens have timed out");

        _totalFirstMessageWaitTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_reversecalls_first_message_wait_seconds_total",
            "seconds",
            "ReverseCall total time spent waiting for first message");
    }

    /// <inheritdoc/>
    public void IncrementPendingStreamWrites()
    {
        _currentPendingStreamWrites.Inc();
        _currentPendingStreamWritesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void DecrementPendingStreamWrites()
    {
        _currentPendingStreamWrites.Dec();
        _currentPendingStreamWritesOtel.Add(-1);
    }

    /// <inheritdoc/>
    public void IncrementTotalStreamWrites()
    {
        _totalStreamWrites.Inc();
        _totalStreamWritesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalStreamWriteBytes(int writtenBytes)
    {
        _totalStreamWriteBytes.Inc();
        _totalStreamWriteBytesOtel.Add(writtenBytes);
    }

    /// <inheritdoc/>
    public void AddToTotalStreamWriteWaitTime(TimeSpan waitTime)
    {
        _totalStreamWriteWaitTime.Inc(waitTime.TotalSeconds);
        _totalStreamWriteWaitTimeOtel.Add(waitTime.TotalSeconds);
    }

    /// <inheritdoc/>
    public void AddToTotalStreamWriteTime(TimeSpan writeTime)
    {
        _totalStreamWriteTime.Inc(writeTime.TotalSeconds);
        _totalStreamWriteTimeOtel.Add(writeTime.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalStreamReads()
    {
        _totalStreamReads.Inc();
        _totalStreamReadsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalStreamReadBytes(int writtenBytes)
    {
        _totalStreamReadBytes.Inc(writtenBytes);
        _totalStreamReadBytesOtel.Add(writtenBytes);
    }

    /// <inheritdoc/>
    public void IncrementTotalPingsSent()
    {
        _totalPingsSent.Inc();
        _totalPingsSentOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalPongsReceived()
    {
        _totalPongsReceived.Inc();
        _totalPongsReceivedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalKeepaliveTokenResets()
    {
        _totalKeepaliveResets.Inc();
        _totalKeepaliveResetsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalKeepaliveTimeouts()
    {
        _totalKeepaliveTimeouts.Inc();
        _totalKeepaliveTimeoutsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalWaitForFirstMessageTime(TimeSpan waitTime)
    {
        _totalFirstMessageWaitTime.Inc(waitTime.TotalSeconds);
        _totalFirstMessageWaitTimeOtel.Add(waitTime.TotalSeconds);
    }
}
