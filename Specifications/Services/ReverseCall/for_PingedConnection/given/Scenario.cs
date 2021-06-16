// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Callbacks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given
{
    public class Scenario
    {
        readonly List<Step> _steps = new();
        Scenario() { }

        public static Scenario New(Action<Builder> build)
        {
            var scenario = new Scenario();

            build(new Builder(scenario));

            var error = scenario._steps.Select(_ => _.Validate()).FirstOrDefault(_ => _ != default);
            if (error != default) throw error;

            scenario._steps.Sort((a, b) => a.At.Value - b.At.Value);

            return scenario;
        }

        public class Builder
        {
            public Builder(Scenario scenario)
            {
                Receive = new ReceiveBuilder(scenario);
                Send = new SendBuilder(scenario);
            }

            public ReceiveBuilder Receive { get; private set; }
            public SendBuilder Send { get; private set; }

            public class ReceiveBuilder
            {
                readonly Scenario _scenario;
                public ReceiveBuilder(Scenario scenario) => _scenario = scenario;

                public AtSetter Message(a_message message)
                    => _scenario.AddStep(new ReceiveMessageStep { Message = message });

                public AtSetter Exception(Exception exception)
                    => _scenario.AddStep(new ReceiveExceptionStep { Exception = exception });
            }

            public class SendBuilder
            {
                readonly Scenario _scenario;
                public SendBuilder(Scenario scenario) => _scenario = scenario;

                public AtSetter Message(a_message message)
                    => _scenario.AddStep(new SendMessageStep { Message = message });
            }
        }

        public class AtSetter
        {
            readonly Step _step;
            public AtSetter(Step step) => _step = step;

            public void AtTime(int time) => _step.At = time;
        }

        public abstract class Step
        {
            public int? At { get; set; }

            public SpecificationUsageException Validate()
            {
                if (!At.HasValue) return new SpecificationUsageException("The time was not set for a step");

                return default;
            }

            public abstract void Execute(Scenario scenario);
        }

        public class ReceiveMessageStep : Step
        {
            public a_message Message { get; set; }

            public override void Execute(Scenario scenario)
            {
                scenario._nextPendingMessage.SetResult(new(true, Message));
            }
        }

        public class ReceiveExceptionStep : Step
        {
            public Exception Exception { get; set; }

            public override void Execute(Scenario scenario)
            {
                scenario._nextPendingMessage.SetException(Exception);
            }
        }

        public class SendMessageStep : Step
        {
            public a_message Message { get; set; }

            public override void Execute(Scenario scenario)
            {
                scenario._connectionWriter.WriteAsync(Message);
            }
        }

        AtSetter AddStep(Step step)
        {
            _steps.Add(step);
            return new(step);
        }

        public void Simulate(
            RequestId requestId,
            ServerCallContext context,
            IConvertReverseCallMessages<a_message, a_message, object, object, object, object> messageConverter,
            IMetricsCollector metrics,
            ILoggerFactory loggerFactory)
        {
            _simulatedTime = 0;
            _writtenMessages = new();
            _receivedMessages = new();
            _nextPendingMessage = new();
            _readMessageException = new(-1, null);
            _scheduledCallbacks = new();
            _refreshedTokenTimes = new();

            var fakeKeepaliveDeadline = new SimulatedKeepaliveDeadline(this);
            var fakeCallbackScheduler = new SimulatedCallbackScheduler(this);

            var connection = new PingedConnection<a_message, a_message, object, object, object, object>(
                requestId,
                new SimulatedStreamReader(this),
                new SimulatedStreamWriter(this),
                context,
                messageConverter,
                fakeKeepaliveDeadline,
                fakeCallbackScheduler,
                metrics,
                loggerFactory);

            _connectionWriter = connection.ClientStream;
            ConnectionCancellationToken = connection.CancellationToken;

            var reader = ReadAllConnectionOutputs(connection);

            var time = 0;
            var currentStepIndex = 0;
            while (currentStepIndex < _steps.Count)
            {
                _simulatedTime = time;
                fakeKeepaliveDeadline.SimulateTokenTimeout();
                fakeCallbackScheduler.SimulateCallbacks();

                while (currentStepIndex < _steps.Count && _steps[currentStepIndex].At.Value == time)
                {
                    _steps[currentStepIndex].Execute(this);
                    currentStepIndex++;
                }
                time++;
            }
            _nextPendingMessage.SetResult(new(false, null));

            reader.GetAwaiter().GetResult();

            Thread.Sleep(10);
        }

        IAsyncStreamWriter<a_message> _connectionWriter;

        public CancellationToken ConnectionCancellationToken { get; private set; }

        int _simulatedTime;
        List<Tuple<int, a_message>> _writtenMessages;
        List<Tuple<int, a_message>> _receivedMessages;
        Tuple<int, Exception> _readMessageException;
        TaskCompletionSource<Tuple<bool, a_message>> _nextPendingMessage;
        List<Tuple<int, TimeSpan>> _scheduledCallbacks;
        List<int> _refreshedTokenTimes;

        public Exception RuntimeStreamMoveNextException => _readMessageException.Item2;
        public IEnumerable<TimeSpan> ScheduledCallbacks => _scheduledCallbacks.Select(_ => _.Item2);
        public IEnumerable<int> RefreshedTokenTimes => _refreshedTokenTimes;
        public IEnumerable<a_message> WrittenMessages => _writtenMessages.Select(_ => _.Item2);
        public IEnumerable<int> WrittenMessageTimes => _writtenMessages.Select(_ => _.Item1);


        class SimulatedStreamReader : IAsyncStreamReader<a_message>
        {
            readonly Scenario _scenario;
            public SimulatedStreamReader(Scenario scenario) => _scenario = scenario;

            public a_message Current { get; private set; }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = await _scenario._nextPendingMessage.Task.ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (result.Item1)
                    {
                        Current = result.Item2;
                    }
                    return result.Item1;
                }
                finally
                {
                    _scenario._nextPendingMessage = new();
                }
            }
        }

        class SimulatedStreamWriter : IAsyncStreamWriter<a_message>
        {
            readonly Scenario _scenario;
            public SimulatedStreamWriter(Scenario scenario) => _scenario = scenario;

            public WriteOptions WriteOptions { get; set; }

            public Task WriteAsync(a_message message)
            {
                _scenario._writtenMessages.Add(new(_scenario._simulatedTime, message));
                return Task.CompletedTask;
            }
        }

        class SimulatedKeepaliveDeadline : ICancelTokenIfDeadlineIsMissed
        {
            readonly CancellationTokenSource _source = new();
            readonly Scenario _scenario;
            int _tokenCancellationTime = int.MaxValue;
            public SimulatedKeepaliveDeadline(Scenario scenario) => _scenario = scenario;

            public CancellationToken Token => _source.Token;

            public void Dispose()
                => _source.Dispose();

            public void RefreshDeadline(TimeSpan nextRefreshBefore)
            {
                _scenario._refreshedTokenTimes.Add(_scenario._simulatedTime);
                _tokenCancellationTime = _scenario._simulatedTime + (int)nextRefreshBefore.TotalSeconds;
            }

            public void SimulateTokenTimeout()
            {
                if (_scenario._simulatedTime >= _tokenCancellationTime)
                {
                    _source.Cancel();
                }
            }
        }

        class SimulatedCallbackScheduler : ICallbackScheduler
        {
            readonly List<ScheduledCallback> _callbacks = new();
            readonly Scenario _scenario;
            public SimulatedCallbackScheduler(Scenario scenario) => _scenario = scenario;
            public IDisposable ScheduleCallback(Action callback, TimeSpan interval)
            {
                _scenario._scheduledCallbacks.Add(new(_scenario._simulatedTime, interval));
                _callbacks.Add(new ScheduledCallback(callback, interval));
                SimulateCallbacks();
                return null;
            }

            public void SimulateCallbacks()
            {
                foreach (var callback in _callbacks)
                {
                    callback.SimulateCallback(_scenario._simulatedTime);
                }
            }

            class ScheduledCallback
            {
                readonly Action _callback;
                readonly int _interval;
                int _lastCalled = int.MinValue;

                public ScheduledCallback(Action callback, TimeSpan interval)
                {
                    _callback = callback;
                    _interval = (int)interval.TotalSeconds;
                }

                public void SimulateCallback(int simulatedTime)
                {
                    if (simulatedTime >= _lastCalled + _interval)
                    {
                        try
                        {
                            _callback();
                            _lastCalled = simulatedTime;
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        async Task ReadAllConnectionOutputs(PingedConnection<a_message, a_message, object, object, object, object> connection)
        {
            try
            {
                while (await connection.RuntimeStream.MoveNext(connection.CancellationToken).ConfigureAwait(false))
                {
                    _receivedMessages.Add(new(_simulatedTime, connection.RuntimeStream.Current));
                }
            }
            catch (Exception exception)
            {
                _readMessageException = new(_simulatedTime, exception);
            }
        }
    }
}
