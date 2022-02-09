// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

#pragma warning disable CA1720

/// <summary>
/// Represents the types that can be converted to when storing a Projection read model copy in MongoDB.
/// </summary>
public enum ConversionBSONType : ushort
{
    /// <summary>
    /// Don't apply any conversion.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Convert a Date to a BSON Date.
    /// </summary>
    DateAsDate,
    
    /// <summary>
    /// Convert a Date to a BSON Array with .NET ticks and offset in minutes as elements.
    /// </summary>
    DateAsArray,
    
    /// <summary>
    /// Convert a Date to a BSON Document with "DateTime": BSON Date, "Ticks": .NET ticks and "Offset": offset in minutes as properties.
    /// </summary>
    DateAsDocument,
    
    /// <summary>
    /// Convert a Date to a BSON String using the Newtonsoft serializer to string.
    /// </summary>
    DateAsString,
    
    /// <summary>
    /// Convert a Date to a BSON Int64 with value from .NET ticks.
    /// </summary>
    DateAsInt64,
    
    /// <summary>
    /// Convert a Guid to a BSON Binary Guid with standard representation.
    /// </summary>
    GuidAsStandardBinary,
    
    /// <summary>
    /// Convert a Guid to a BSON Binary Guid with C# legacy representation.
    /// </summary>
    GuidAsCsharpLegacyBinary,
    
    /// <summary>
    /// Convert a Guid to a BSON String.
    /// </summary>
    GuidAsString,
}
