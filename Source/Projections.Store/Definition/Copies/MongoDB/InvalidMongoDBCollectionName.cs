// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when attempting to construct a <see cref="CollectionName"/> from a <see cref="string"/> that is not a valid MongoDB collection name.
/// </summary>
public class InvalidMongoDBCollectionName : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMongoDBCollectionName"/> class.
    /// </summary>
    /// <param name="name">The name that is invalid.</param>
    /// <param name="reason">The reason why the name is invalid.</param>
    public InvalidMongoDBCollectionName(string name, string reason)
        : base($"The collection name '{name}' is invalid because {reason}")
    {
    }
}
