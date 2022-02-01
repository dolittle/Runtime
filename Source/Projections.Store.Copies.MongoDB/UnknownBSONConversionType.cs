// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// The exception that gets thrown when attempting to convert a <see cref="BsonValue"/> using an unknown <see cref="ConversionBSONType"/>.
/// </summary>
public class UnknownBSONConversionType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownBSONConversionType"/> class.
    /// </summary>
    /// <param name="conversion">The unknown conversion type.</param>
    public UnknownBSONConversionType(ConversionBSONType conversion)
        : base($"Cannot convert value using the BSON conversion {conversion}")
    {
    }
}
