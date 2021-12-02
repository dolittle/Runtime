// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.Rudimentary;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.Concepts;

/// <summary>
/// An implementation of <see cref="IValueParser"/> that parses implementations of <see cref="ConceptAs{TValue}"/> of type <see cref="string"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class StringConceptParser<T> : ConceptParser<T, string>
    where T : ConceptAs<string>
{
    /// <inheritdoc />
    protected override string Parse(string value, CultureInfo culture) => value;
}