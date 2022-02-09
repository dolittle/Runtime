// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// The exception that is thrown when it is not possible to apply a <see cref="ConversionBSONType"/> to a <see cref="BsonValue"/>.
/// </summary>
public class CannotConvertValueUsingConversion : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotConvertValueUsingConversion"/> class.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="conversion">The conversion to be applied.</param>
    public CannotConvertValueUsingConversion(BsonValue value, ConversionBSONType conversion)
        : base($"Cannot convert value '{value}' of type {value.BsonType} using conversion {conversion}")
    {
    }
}
