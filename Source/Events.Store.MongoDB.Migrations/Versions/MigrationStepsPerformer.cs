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
    public class MigrationStepsPerformer : IPerformMigrationStepsInOrder
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
                session.StartTransaction();
                var steps = createSteps(session, cts.Token);
                foreach (var step in steps)
                {
                    await step.ConfigureAwait(false);
                }
                await session.CommitTransactionAsync(cancellationToken: cts.Token).ConfigureAwait(false);
                watch.Stop();
                _logger.LogInformation("Performing migration steps took {Time}", watch.Elapsed);
                return Try.Succeeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while performing migration steps");
                cts.Cancel();
                if (session == default) return ex;
                await session.AbortTransactionAsync().ConfigureAwait(false);
                return ex;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
}