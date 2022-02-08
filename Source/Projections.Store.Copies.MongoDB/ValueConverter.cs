// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

#pragma warning disable CS8509

/// <summary>
/// Represents an implementation of <see cref="IValueConverter"/>.
/// </summary>
public class ValueConverter : IValueConverter
{
    /// <inheritdoc />
    public BsonValue Convert(BsonValue value, ConversionBSONType conversion)
    {
        switch (conversion)
        {
            case ConversionBSONType.None:
                return value;
            case ConversionBSONType.DateAsDate:
            case ConversionBSONType.DateAsArray:
            case ConversionBSONType.DateAsDocument:
            case ConversionBSONType.DateAsString:
            case ConversionBSONType.DateAsInt64:
                return ConvertToDate(value, conversion);
            case ConversionBSONType.GuidAsStandardBinary:
            case ConversionBSONType.GuidAsCsharpLegacyBinary:
            case ConversionBSONType.GuidAsString:
                return ConvertToGuid(value, conversion);
            default:
                throw new UnknownBSONConversionType(conversion);
        }
    }

    static BsonValue ConvertToDate(BsonValue value, ConversionBSONType conversion)
    {
        if (value is BsonDateTime dateTimeValue)
        {
            return ConvertDateTo(new DateTimeOffset(dateTimeValue.ToUniversalTime(), TimeSpan.Zero), conversion);
        }

        if (value is BsonString stringValue)
        {
            return ConvertDateTo(DateTimeOffset.Parse(stringValue.Value, CultureInfo.InvariantCulture), conversion);
        }

        throw new CannotConvertValueUsingConversion(value, conversion);
    }

    static BsonValue ConvertDateTo(DateTimeOffset value, ConversionBSONType conversion)
        => conversion switch
        {
            ConversionBSONType.DateAsDate => new BsonDateTime(value.UtcDateTime),
            ConversionBSONType.DateAsArray => new BsonArray
            {
                new BsonInt64(value.Ticks),
                new BsonInt32((int) value.Offset.TotalMinutes),
            },
            ConversionBSONType.DateAsDocument => new BsonDocument
            {
                new BsonElement("DateTime", BsonUtils.ToMillisecondsSinceEpoch(value.UtcDateTime)),
                new BsonElement("Ticks", new BsonInt64(value.Ticks)),
                new BsonElement("Offset", new BsonInt32((int) value.Offset.TotalMinutes)),
            },
            ConversionBSONType.DateAsString => JsonConvert.ToString(value),
            ConversionBSONType.DateAsInt64 => new BsonInt64(value.Ticks),
        };

    static BsonValue ConvertToGuid(BsonValue value, ConversionBSONType conversion)
    {
        if (value is BsonString stringValue)
        {
            return ConvertGuidTo(Guid.Parse(stringValue.Value), conversion);
        }
        
        throw new CannotConvertValueUsingConversion(value, conversion);
    }

    static BsonValue ConvertGuidTo(Guid value, ConversionBSONType conversion)
        => conversion switch
        {
            ConversionBSONType.GuidAsStandardBinary => new BsonBinaryData(value, GuidRepresentation.Standard),
            ConversionBSONType.GuidAsCsharpLegacyBinary => new BsonBinaryData(value, GuidRepresentation.CSharpLegacy),
            ConversionBSONType.GuidAsString => new BsonString(value.ToString()),
        };
}
