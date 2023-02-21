// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.MongoDB;

public static class UpsertHelpers<TK, TV> where TV : class
{
    static readonly FilterDefinitionBuilder<TV> _filterDefinitionBuilder = new();

    public static async Task UpsertMany(IMongoCollection<TV> collection, IReadOnlyDictionary<TK, TV> entities)
    {
        if (entities.Count == 0)
        {
            return;
        }

        await collection.BulkWriteAsync(entities.Select(e => new ReplaceOneModel<TV>(_filterDefinitionBuilder.Eq("_id", e.Key), e.Value)
        {
            IsUpsert = true,
        }));
    }
}
