// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when an invalid Projection read model copy to MongoDB field conversion was received.
/// </summary>
public class InvalidMongoDBFieldConversion : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMongoDBFieldConversion"/> class.
    /// </summary>
    /// <param name="field">The field to convert.</param>
    /// <param name="conversionType">The conversion type specified.</param>
    public InvalidMongoDBFieldConversion(string field, ProjectionCopyToMongoDB.Types.BSONType conversionType)
        : base($"Received invalid {nameof(CopyToMongoDBSpecification)} conversion type {conversionType} for field {field}")
    {
    }
}
