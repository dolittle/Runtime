// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Extension methods for <see cref="FilterDefinitionBuilder{TDocument}"/>.
/// </summary>
public static class FilterDefinitionBuilderExtensions
{
    /// <summary>Creates an equality filter that considers a GUID to be equal to its string representation.</summary>
    /// <param name="_">The <see cref="FilterDefinitionBuilder{TDocument}"/>.</param>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>An equality filter.</returns>
    public static FilterDefinition<TDocument> EqStringOrGuid<TDocument>(
        this FilterDefinitionBuilder<TDocument> _,
        Expression<Func<TDocument, string>> field,
        string value)
        => new StringOrGuidFilterDefinition<TDocument>(new ExpressionFieldDefinition<TDocument, string>(field), value);
}
