// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Rudimentary;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.Concepts;

/// <summary>
/// An implementation of <see cref="IValueParser"/> that parses implementations of <see cref="ConceptAs{TValue}"/> of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GuidConceptParser<T> : ConceptParser<T, Guid>
    where T : ConceptAs<Guid>
{
    /// <inheritdoc />
    protected override Guid Parse(string value, CultureInfo culture) => Guid.Parse(value);
}