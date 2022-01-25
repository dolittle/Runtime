// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the types that can be converted to when storing a Projection read model copy in MongoDB.
/// </summary>
public enum ConversionBSONType
{
    Date = 0,
    Timestamp = 1,
    Binary = 2,
}
