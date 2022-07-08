// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Defines a system that can convert a <see cref="BsonValue"/> to another <see cref="BsonValue"/> based on a <see cref="ConversionBSONType"/>.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// Converts a <see cref="BsonValue"/> using the specified <see cref="ConversionBSONType"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="conversion">The conversion to apply.</param>
    /// <returns>The converted <see cref="BsonValue"/>.</returns>
    BsonValue Convert(BsonValue value, ConversionBSONType conversion);
}
