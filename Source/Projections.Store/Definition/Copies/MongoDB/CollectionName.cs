// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the name of the collection to store MongoDB read model copies in.
/// </summary>
/// <param name="Value">The name of the collection.</param>
public record CollectionName(string Value) : ConceptAs<string>(Value);
