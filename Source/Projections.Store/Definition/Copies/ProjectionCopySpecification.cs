// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies;

/// <summary>
/// Represents the specification of read model copies to produce.
/// </summary>
/// <param name="MongoDB">The specification of MongoDB read model copies.</param>
public record ProjectionCopySpecification(
    CopyToMongoDBSpecification MongoDB);
