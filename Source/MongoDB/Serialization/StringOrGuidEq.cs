// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Dolittle.Runtime.MongoDB.Serialization;

/// <summary>
/// Represents an implementation of <see cref="FilterDefinition{TDocument}"/> for a string or guid.
/// </summary>
/// <typeparam name="TDocument">The <see cref="Type"/> of the document.</typeparam>
public class StringOrGuidEq<TDocument> : FilterDefinition<TDocument>
{
    readonly FieldDefinition<TDocument, string> _field;
    readonly string _value;
    
    /// <summary>
    /// Initialize a new instance of the <see cref="StringOrGuidEq{TDocument}"/> class.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="value">The guid or string value.</param>
    public StringOrGuidEq(FieldDefinition<TDocument, string> field, string value)
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
        writer.WriteStartDocument();
        writer.WriteName("$in");
        writer.WriteStartArray();
        writer.WriteString(_value);
        if (Guid.TryParse(_value, out var guidValue))
        {
            writer.WriteBinaryData(new BsonBinaryData(guidValue, GuidRepresentation.Standard));
        }
        writer.WriteEndArray();
        writer.WriteEndDocument();
        writer.WriteEndDocument();
        return document;
    }
}
