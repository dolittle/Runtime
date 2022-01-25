// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the types that can be converted to when storing a Projection read model copy in MongoDB.
/// </summary>
public enum ConversionBSONType : ushort
{
    /// <summary>
    /// Convert to a BSON Date.
    /// </summary>
    Date = 0,
    
    /// <summary>
    /// Convert to a BSON Timestamp.
    /// </summary>
    Timestamp,
    
    /// <summary>
    /// Convert to a BSON Binary.
    /// </summary>
    Binary
}
