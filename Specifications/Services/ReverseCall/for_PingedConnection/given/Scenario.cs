// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given
{
    public class Scenario
    {
        readonly List<Step> _steps = new();
        Scenario() {}

        public static Scenario New(Action<Builder> build)
        {
            var scenario = new Scenario();
            
            build(new Builder(scenario));

            var error = scenario._steps.Select(_ => _.Validate()).FirstOrDefault(_ => _ != default);
            if (error != default) throw error;
            
            scenario._steps.Sort((a, b) => a.At.Value - b.At.Value);

            var lastStep = scenario._steps.LastOrDefault();
            var finalStepTime = 0;
            if (lastStep != default) finalStepTime = lastStep.At.Value + 100;

            scenario._steps.Add(new FinalStep{ At = finalStepTime });
            
            return scenario;
        }

        public class Builder
        {
            public Builder(Scenario scenario)
            {
                Receive = new ReceiveBuilder(scenario);
            }

            public ReceiveBuilder Receive { get; private set; }

            public class ReceiveBuilder
            {
                readonly Scenario _scenario;
                public ReceiveBuilder(Scenario scenario) => _scenario = scenario;

                public AtSetter Message(a_message message)
                    => _scenario.AddStep(new ReceiveMessageStep{ Message = message });

                public AtSetter Exception(Exception exception)
                    => _scenario.AddStep(new ReceiveExceptionStep{ Exception = exception });
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

        public class FinalStep : Step
        {
            public override void Execute(Scenario scenario)
            {
                scenario._nextPendingMessage.SetResult(new(false, null));
                scenario._nextPendingMessage = new();
            }
        }

        public class ReceiveMessageStep : Step
        {
            public a_message Message { get; set; }

            public override void Execute(Scenario scenario)
            {
                scenario._nextPendingMessage.SetResult(new(true, Message));
                scenario._nextPendingMessage = new();
            }
        }

        public class ReceiveExceptionStep : Step
        {
            public Exception Exception { get; set; }

            public override void Execute(Scenario scenario)
            {
                scenario._nextPendingMessage.SetException(Exception);
                scenario._nextPendingMessage = new();
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

            var fakeKeepaliveDeadline = new SimulatedKeepaliveDeadline(this);

            var connection = new PingedConnection<a_message, a_message, object, object, object, object>(
                requestId,
                new SimulatedStreamReader(this),
                new SimulatedStreamWriter(this),
                context,
                messageConverter,
                fakeKeepaliveDeadline,
                metrics,
                loggerFactory);

            ConnectionCancellationToken = connection.CancellationToken;

            var reader = ReadAllConnectionOutputs(connection);

            foreach (var step in _steps)
            {
                _simulatedTime = step.At.Value;
                step.Execute(this);
            }

            reader.GetAwaiter().GetResult();

            Thread.Sleep(10);
        }

        public CancellationToken ConnectionCancellationToken { get; private set; }

        int _simulatedTime;
        List<Tuple<int, a_message>> _writtenMessages;
        List<Tuple<int, a_message>> _receivedMessages;
        Tuple<int, Exception> _readMessageException;
        TaskCompletionSource<Tuple<bool, a_message>> _nextPendingMessage;

        public Exception RuntimeStreamMoveNextException => _readMessageException.Item2;

        class SimulatedStreamReader : IAsyncStreamReader<a_message>
        {
            readonly Scenario _scenario;
            public SimulatedStreamReader(Scenario scenario) => _scenario = scenario;

            public a_message Current { get; private set; }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                var result = await _scenario._nextPendingMessage.Task.ConfigureAwait(false);
                if (result.Item1)
                {
                    Current = result.Item2;
                }
                return result.Item1;
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
            public SimulatedKeepaliveDeadline(Scenario scenario) => _scenario = scenario;

            public CancellationToken Token => _source.Token;

            public void Dispose()
                => _source.Dispose();

            public void RefreshDeadline(TimeSpan nextRefreshBefore)
            {
                // TODO: Implement with fake time
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
