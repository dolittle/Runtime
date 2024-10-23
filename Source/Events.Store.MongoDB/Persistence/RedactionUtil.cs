// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Redactions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

public static class RedactionUtil
{
    /// <summary>
    /// This will redact specific personal data from the event log
    /// </summary>
    /// <param name="session"></param>
    /// <param name="collection"></param>
    /// <param name="redactions"></param>
    /// <param name="cancellationToken"></param>
    public static async Task RedactEvents(IClientSessionHandle session, IMongoCollection<Events.Event> collection,
        IReadOnlyCollection<Redaction> redactions, CancellationToken cancellationToken)
    {
        if (redactions.Count == 0) return;

        var updateOperations = redactions.Select(redaction => redaction.CreateUpdateModel()).ToList();

        await collection.BulkWriteAsync(session, updateOperations, new BulkWriteOptions
        {
            IsOrdered = true,
            BypassDocumentValidation = false
        }, cancellationToken: cancellationToken);
    }

    private static UpdateManyModel<Events.Event> CreateUpdateModel(this Redaction redaction)
    {
        var (updateDefinition, hasUpdateFilter) = CreateUpdateDefinition(redaction);
        var matchesEventFilter = CreateFilter(redaction);
        var filter = Builders<Events.Event>.Filter.And(matchesEventFilter, hasUpdateFilter);
        return new UpdateManyModel<Events.Event>(filter, updateDefinition);
    }

    static (UpdateDefinition<Events.Event> updateDefinition, FilterDefinition<Events.Event> alreadyUpdatedFilter) CreateUpdateDefinition(this Redaction redaction)
    {
        var updates = new List<UpdateDefinition<Events.Event>>();
        var filters = new List<FilterDefinition<Events.Event>>();

        foreach (var (field, value) in redaction.Details.RedactedProperties)
        {
            var asBson = ToBsonValue(value);

            FieldDefinition<Events.Event, BsonValue> contentField = $"Content.{field}";
            if (asBson is null)
            {
                updates.Add(Builders<Events.Event>.Update.Unset(contentField));
                filters.Add(Builders<Events.Event>.Filter.Exists(contentField));
            }
            else
            {
                updates.Add(Builders<Events.Event>.Update.Set(contentField, asBson));
                filters.Add(Builders<Events.Event>.Filter.Ne(contentField, asBson));
            }
        }

        // Point to the redaction event
        updates.Add(Builders<Events.Event>.Update.AddToSet("RedactedBy", redaction.EventLogSequenceNumber.Value));

        var matchesAnyFilter = Builders<Events.Event>.Filter.Or(filters);
        return (Builders<Events.Event>.Update.Combine(updates), matchesAnyFilter);
    }

    static FilterDefinition<Events.Event> CreateFilter(this Redaction redaction)
    {
        return Builders<Events.Event>.Filter.And(
            Builders<Events.Event>.Filter.Lt(evt => evt.EventLogSequenceNumber, redaction.EventLogSequenceNumber.Value),
            Builders<Events.Event>.Filter.Eq(evt => evt.Metadata.EventSource, redaction.EventSourceId.Value),
            Builders<Events.Event>.Filter.Eq(evt => evt.Metadata.TypeId, redaction.TypeId)
        );
    }

    static BsonValue? ToBsonValue(object? element)
    {
        if (element is null)
        {
            return null;
        }

        if (element is JsonElement jsonElement)
        {
            return jsonElement.ToBsonValue();
        }

        switch (element)
        {
            case string str:
                return new BsonString(str);

            case int intValue:
                return new BsonInt32(intValue);

            case long longValue:
                return new BsonInt64(longValue);

            case double doubleValue:
                return new BsonDouble(doubleValue);

            case decimal decimalValue:
                return new BsonDecimal128(decimalValue);

            case bool boolValue:
                return boolValue ? BsonBoolean.True : BsonBoolean.False;

            default:
                return null;
        }
    }


    /// <summary>
    /// Converts supported JSON values to BSON values
    /// Does not support arrays or objects
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    static BsonValue? ToBsonValue(this JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                var str = element.GetString();

                return new BsonString(str);

            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intValue))
                {
                    return new BsonInt32(intValue);
                }

                if (element.TryGetInt64(out var longValue))
                {
                    return new BsonInt64(longValue);
                }

                if (element.TryGetDouble(out var doubleValue))
                {
                    return new BsonDouble(doubleValue);
                }

                if (element.TryGetDecimal(out var decimalValue))
                {
                    return new BsonDecimal128(decimalValue);
                }

                throw new ArgumentException("Unsupported numeric type");

            case JsonValueKind.True:
                return BsonBoolean.True;

            case JsonValueKind.False:
                return BsonBoolean.False;

            case JsonValueKind.Null:
            default:
                return null;
        }
    }
}
