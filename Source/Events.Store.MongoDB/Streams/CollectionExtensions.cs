// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

public static class CollectionExtensions
{
    public static async Task<ulong> CountDocuments<TEvent>(this IMongoCollection<TEvent> collection, IClientSessionHandle session, CancellationToken cancellationToken) =>
        (ulong) await collection.CountDocumentsAsync(
            session,
            Builders<TEvent>.Filter.Empty,
            cancellationToken: cancellationToken).ConfigureAwait(false);
}
