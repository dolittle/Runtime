// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents an implementation of <see cref="FilterDefinition{TDocument}"/> that considers a GUID to be equal to its string representations.
/// </summary>
/// <typeparam name="TDocument">The <see cref="Type"/> of the document.</typeparam>
/// <remarks>
/// This serializer was introduced to enable backwards-compatibility in the persisted Event Store from v8 to v6 and v7.
/// </remarks>
public class StringOrGuidFilterDefinition<TDocument> : FilterDefinition<TDocument>
{
    readonly FieldDefinition<TDocument, string> _field;
    readonly string _value;

    /// <summary>
    /// Initialize a new instance of the <see cref="StringOrGuidFilterDefinition{TDocument}"/> class.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="value">The string value.</param>
    public StringOrGuidFilterDefinition(FieldDefinition<TDocument, string> field, string value)
    {
        _field = field;
        _value = value;
    }

    /// <inheritdoc />
    public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
    {
        var renderedFieldDefinition = _field.Render(documentSerializer, serializerRegistry, true);
        var document = new BsonDocument();
        using var writer = new BsonDocumentWriter(document);
        writer.WriteStartDocument();
        writer.WriteName(renderedFieldDefinition.FieldName);
        RenderEqualityQuery(writer);
        writer.WriteEndDocument();
        return document;
    }

    public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider) => Render(documentSerializer, serializerRegistry);

    void RenderEqualityQuery(IBsonWriter writer)
    {
        if (Guid.TryParse(_value, out var guidValue))
        {
            writer.WriteStartDocument();
            writer.WriteName("$in");
            writer.WriteStartArray();
            writer.WriteString(_value);
            writer.WriteBinaryData(new BsonBinaryData(guidValue, GuidRepresentation.Standard));
            writer.WriteEndArray();
            writer.WriteEndDocument();
        }
        else
        {
            writer.WriteString(_value);
        }
    }
}
