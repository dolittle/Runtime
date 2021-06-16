// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Reflection;
using Google.Protobuf.Collections;
using Grpc.Core;

namespace Dolittle.Runtime.Management
{
    /// <summary>
    /// Extensions for observing state and forward onto Grpc streams.
    /// </summary>
    public static class ObserveAndForwardToGrpcExtensions
    {
        /// <summary>
        /// Forward state from a <see cref="ObservableCollection{T}"/> to a Grpc stream.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TEnvelope">Envelope used as container for the stream.</typeparam>
        /// <typeparam name="TTarget">Target type to map to.</typeparam>
        /// <param name="source">The source <see cref="ObservableCollection{T}"/>.</param>
        /// <param name="streamWriter">The <see cref="IServerStreamWriter{T}"/> to write to.</param>
        /// <param name="context"><see cref="ServerCallContext"/>.</param>
        /// <param name="targetProperty">Expression representing the target property on envelope to add mapped result to.</param>
        /// <param name="map">Maping <see cref="Func{TSource, TTarget}"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Forward<TSource, TEnvelope, TTarget>(
            this ObservableCollection<TSource> source,
            IServerStreamWriter<TEnvelope> streamWriter,
            ServerCallContext context,
            Expression<Func<TEnvelope, RepeatedField<TTarget>>> targetProperty,
            Func<TSource, TTarget> map)
            where TEnvelope : new()
        {
            using var autoResetEvent = new AutoResetEvent(false);

            void StateChanged(object sender, NotifyCollectionChangedEventArgs args) => autoResetEvent.Set();

            try
            {
                var propertyInfo = targetProperty.GetPropertyInfo();
                source.CollectionChanged += StateChanged;

                context.CancellationToken.ThrowIfCancellationRequested();

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    var response = new TEnvelope();
                    var repeatedFields = propertyInfo.GetValue(response) as RepeatedField<TTarget>;
                    var mapped = source.Select(_ => map(_));
                    repeatedFields.AddRange(mapped);

                    await streamWriter.WriteAsync(response).ConfigureAwait(false);

                    autoResetEvent.WaitOne();
                }
            }
            finally
            {
                source.CollectionChanged -= StateChanged;
            }
        }
    }
}