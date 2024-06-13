// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.CLI.Runtime.Aggregates;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.Aggregates;

/// <summary>
/// An implementation of <see cref="IValueParser"/> that parses instances of <see cref="AggregateRootIdOrAlias"/>.
/// </summary>
public class AggregateRootIdOrAliasParser : IValueParser
{
    /// <inheritdoc />
    public Type TargetType => typeof(AggregateRootIdOrAlias);

    /// <inheritdoc />
    public object Parse(string argName, string value, CultureInfo culture)
        => Guid.TryParse(value, out var aggregateRootId)
            ? new AggregateRootIdOrAlias(aggregateRootId)
            : new AggregateRootIdOrAlias(new AggregateRootAlias(value));
}
