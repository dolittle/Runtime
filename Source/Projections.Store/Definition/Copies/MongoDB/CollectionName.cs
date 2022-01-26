// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the name of the collection to store MongoDB read model copies in.
/// </summary>
public record CollectionName : ConceptAs<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionName"/> record.
    /// </summary>
    /// <param name="name">The collection name.</param>
    public CollectionName(string name)
        : base(name)
    {
        ThrowIfNameIsEmpty(name);
        ThrowIfNameIsTooLong(name);
        ThrowIfNameContainsIllegalCharacter(name);
        ThrowIfNameBeginsWithSystemPrefix(name);
    }

    /// <summary>
    /// Gets the <see cref="CollectionName"/> value when it is not set.
    /// </summary>
    public static readonly CollectionName NotSet = "Not Set";

    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to a <see cref="CollectionName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> representation of the collection name.</param>
    /// <returns>The converted <see cref="CollectionName"/> concept.</returns>
    public static implicit operator CollectionName(string name) => new(name);

    static void ThrowIfNameIsEmpty(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidMongoDBCollectionName(name, "it cannot be empty");
        }
    }

    static void ThrowIfNameIsTooLong(string name)
    {
        if (Encoding.UTF8.GetByteCount(name) >= 120)
        {
            throw new InvalidMongoDBCollectionName(name, "it must be at most 120 bytes long");
        }
    }
    
    static void ThrowIfNameContainsIllegalCharacter(string name)
    {
        if (name.Contains('$'))
        {
            throw new InvalidMongoDBCollectionName(name, "it cannot contain the character '$'");
        }
        if (name.Contains('\0'))
        {
            throw new InvalidMongoDBCollectionName(name, "it cannot contain the null character");
        }
    }

    static void ThrowIfNameBeginsWithSystemPrefix(string name)
    {
        if (name.StartsWith("system.", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new InvalidMongoDBCollectionName(name, "it cannot start with 'system.'");
        }
    }
}
