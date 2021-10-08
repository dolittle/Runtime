// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions
{
    public interface IPerformMigrationStepsInOrder
    {
        Task<Try> Perform(IMongoDatabase database, Func<IClientSessionHandle, CancellationToken, IEnumerable<Task>> createSteps);
    }
}