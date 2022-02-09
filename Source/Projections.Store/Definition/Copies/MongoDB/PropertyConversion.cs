// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the specification of a conversion for a read model copy to store in MongoDB.
/// </summary>
/// <param name="Property">The name of the property.</param>
/// <param name="Conversion">The conversion to apply.</param>
/// <param name="ShouldRename">A value indicating whether or not the property should be renamed.</param>
/// <param name="RenameTo">The property name to rename to.</param>
/// <param name="Children">Conversions to apply to child properties of this property.</param>
public record PropertyConversion(
    ProjectionProperty Property,
    ConversionBSONType Conversion,
    bool ShouldRename,
    ProjectionProperty RenameTo,
    PropertyConversion[] Children);
