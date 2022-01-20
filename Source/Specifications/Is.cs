// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specifications;

/// <summary>
/// Helps chain simple <see cref="Specification{T}"/> together.
/// </summary>
public static class Is
{
    /// <summary>
    /// Creates a Not rule based on the rule passed in.
    /// </summary>
    /// <typeparam name="T">Type of the instance that the rule is to be evaluated against.</typeparam>
    /// <param name="rule">The rule being extended.</param>
    /// <returns>A Not{T} rule">.</returns>
    public static Specification<T> Not<T>(Specification<T> rule) => new Negative<T>(rule);
}