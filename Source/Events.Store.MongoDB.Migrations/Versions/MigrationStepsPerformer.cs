// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions
{
    public class MigrationStepsPerformer : IPerformMigrationSteps
    {
        readonly ILogger<MigrationStepsPerformer> _logger;

        public MigrationStepsPerformer(ILogger<MigrationStepsPerformer> logger)
        {
            _logger = logger;
        }

        public async Task<Try> Perform(IMongoDatabase database, Func<IClientSessionHandle, CancellationToken, IEnumerable<Task>> createSteps)
        {
            IClientSessionHandle session = default;
            using var cts = new CancellationTokenSource();
            try
            {
                session = await database.Client.StartSessionAsync(cancellationToken: cts.Token).ConfigureAwait(false);
                _logger.LogInformation("Start performing migration steps");
                var watch = new Stopwatch();
                watch.Start();
                var steps = createSteps(session, cts.Token);
                await Task.WhenAll(steps).ConfigureAwait(false);
                watch.Stop();
                _logger.LogInformation("Migration finished after {Time}", watch.Elapsed);
                return Try.Succeeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while performing migration steps");
                if (session == default) return ex;
                await session.AbortTransactionAsync(cancellationToken: cts.Token).ConfigureAwait(false);
                return ex;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
}