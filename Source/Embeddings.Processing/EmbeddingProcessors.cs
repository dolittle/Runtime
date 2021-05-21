// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingProcessors"/>.
    /// </summary>
    [Singleton]
    public class EmbeddingProcessors : IEmbeddingProcessors
    {
        readonly ConcurrentDictionary<EmbeddingId, Dictionary<TenantId, IEmbeddingProcessor>> _processors = new();
        readonly ITenants _tenants;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessors"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants"/> to use to get all configured tenants.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public EmbeddingProcessors(ITenants tenants, ILogger logger)
        {
            _tenants = tenants;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try> TryStartEmbeddingProcessorForAllTenants(EmbeddingId embedding, CreateEmbeddingProcessorForTenant factory, CancellationToken cancellationToken)
        {
            if (!TryRegisterAndCreateProcessors(embedding, factory, out var processors, out var error))
            {
                return error;
            }

            var tryStartProcessors = await TryStartAndWaitForAllProcessorsToFinish(processors.Select(_ => _.Value), cancellationToken).ConfigureAwait(false);
            _processors.TryRemove(embedding, out var _);
            return tryStartProcessors;
        }

        /// <inheritdoc/>
        public bool HasEmbeddingProcessors(EmbeddingId embedding) => _processors.ContainsKey(embedding);

        /// <inheritdoc/>
        public bool TryGetEmbeddingProcessorFor(TenantId tenant, EmbeddingId embedding, out IEmbeddingProcessor processor)
        {
            processor = null;
            if (_processors.TryGetValue(embedding, out var processorsByTenant))
            {
                return processorsByTenant.TryGetValue(tenant, out processor);
            }
            return false;
        }

        bool TryRegisterAndCreateProcessors(EmbeddingId embedding, CreateEmbeddingProcessorForTenant createProcessor, out Dictionary<TenantId, IEmbeddingProcessor> processors, out Exception error)
        {
            try
            {
                error = null;
                processors = new Dictionary<TenantId, IEmbeddingProcessor>();
                if (!_processors.TryAdd(embedding, processors))
                {
                    processors = null;
                    error = new EmbeddingProcessorsAlreadyRegistered(embedding);
                    return false;
                }
                foreach (var tenant in _tenants.All)
                {
                    processors.Add(tenant, createProcessor(tenant));
                }
                return true;
            }
            catch (Exception ex)
            {
                processors = null;
                error = ex;
                return false;
            }
        }

        async Task<Try> TryStartAndWaitForAllProcessorsToFinish(IEnumerable<IEmbeddingProcessor> processors, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                var tasks = processors.Select(_ => _.Start(tokenSource.Token)).ToList();
                var finishedTask = await Task.WhenAny(tasks).ConfigureAwait(false);
                tokenSource.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                return await finishedTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                if (!tokenSource.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }
            }
        }
    }
}