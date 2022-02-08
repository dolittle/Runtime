// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IValueConverter"/>.
/// </summary>
public class ValueConverter : IValueConverter
{
    /// <inheritdoc />
    public BsonValue Convert(BsonValue value, ConversionBSONType conversion)
        => conversion switch
        {
            ConversionBSONType.None => value,
            ConversionBSONType.DateAsDate => ConvertToDateAsDate(value),
            ConversionBSONType.GuidAsStandardBinary => ConvertToGuidAsStandardBinary(value),
            _ => throw new UnknownBSONConversionType(conversion),
        };

    static BsonDateTime ConvertToDateAsDate(BsonValue value)
        => value switch
        {
            BsonDateTime dateTimeValue => dateTimeValue,
            BsonString stringValue => new BsonDateTime(DateTimeOffset.Parse(stringValue.Value, CultureInfo.InvariantCulture).UtcDateTime),
            _ => throw new CannotConvertValueUsingConversion(value, ConversionBSONType.DateAsDate),
        };

    static BsonBinaryData ConvertToGuidAsStandardBinary(BsonValue value)
        => value switch
        {
            _ => throw new CannotConvertValueUsingConversion(value, ConversionBSONType.GuidAsStandardBinary),
        };
}
