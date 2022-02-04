// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents the persisted copy to MongoDB property conversion specification of a projection.
/// </summary>
public class ProjectionCopyToMongoDBPropertyConversion
{
    /// <summary>
    /// Gets or sets the property name to be converted.
    /// </summary>
    public string Property { get; set; }
    
    /// <summary>
    /// Gets or sets the conversion to apply to the property.
    /// </summary>
    public ConversionBSONType ConversionType { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether or not to rename the property.
    /// </summary>
    public bool ShouldRename { get; set; }

    /// <summary>
    /// Gets or sets the name to rename the property to.
    /// </summary>
    public string RenameTo { get; set; }

    /// <summary>
    /// Gets or sets child properties to convert on the property.
    /// </summary>
    public IEnumerable<ProjectionCopyToMongoDBPropertyConversion> Children { get; set; }
}
