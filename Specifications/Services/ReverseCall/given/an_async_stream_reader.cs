// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls.given
{
    public class an_async_stream_reader<T> : IAsyncStreamReader<T>
    {
        readonly List<Task<Tuple<bool, T>>> _messages_to_receive = new();
        int _message_pointer;

        private an_async_stream_reader()
        {
        }

        public T Current { get; private set; }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (_message_pointer > _messages_to_receive.Count)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
                return false;
            }

            var result = await _messages_to_receive[_message_pointer].ConfigureAwait(false);
            _message_pointer++;
            Current = result.Item2;

            return result.Item1;
        }

        public static an_async_stream_reader<T> that()
            => new();

        public an_async_stream_reader<T> receives(T message)
        {
            _messages_to_receive.Add(Task.FromResult(new Tuple<bool, T>(true, message)));
            return this;
        }

        public an_async_stream_reader<T> completes()
        {
            _messages_to_receive.Add(Task.FromResult(new Tuple<bool, T>(false, default)));
            return this;
        }

        public an_async_stream_reader<T> throws(Exception exception)
        {
            _messages_to_receive.Add(Task.FromException<Tuple<bool, T>>(exception));
            return this;
        }
    }
}
