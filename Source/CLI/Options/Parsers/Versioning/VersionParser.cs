// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Domain.Platform;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace CLI.Options.Parsers.Versioning;

/// <summary>
/// An implementation of <see cref="IValueParser"/> that parses semantic version values as <see cref="Version"/>.
/// </summary>
public class VersionParser : IValueParser
{
    readonly IVersionConverter _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionParser"/> class.
    /// </summary>
    /// <param name="converter">The version converter to use to convert a <see cref="string"/> to a <see cref="Version"/>.</param>
    public VersionParser(IVersionConverter converter)
    {
        _converter = converter;
    }

    /// <inheritdoc/>
    public Type TargetType => typeof(Version);

    /// <inheritdoc/>
    public object Parse(string? argName, string? value, CultureInfo culture)
    {
        if (value == null)
        {
            throw new InvalidVersionString("");
        }

        return _converter.FromString(value);
    }
}
