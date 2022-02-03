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
    /// Convert to a BSON Date.
    /// </summary>
    Date,
    
    /// <summary>
    /// Convert to a BSON Binary Guid.
    /// </summary>
    Guid 
}
