// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Configuration.Files;

/// <summary>
/// Represents an implementation of <see creF="IConfigurationFileParsers"/>.
/// </summary>
public class ConfigurationFileParsers : IConfigurationFileParsers
{
    readonly IEnumerable<ICanParseConfigurationFile> _parsers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationFileParsers"/> class.
    /// </summary>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for finding parsers.</param>
    /// <param name="container"><see cerf="IContainer"/> used to get instances.</param>
    public ConfigurationFileParsers(ITypeFinder typeFinder, IContainer container)
    {
        _parsers = typeFinder
            .FindMultiple<ICanParseConfigurationFile>()
            .Select(_ => container.Get(_) as ICanParseConfigurationFile);
    }

    /// <inheritdoc/>
    public object Parse(Type type, string filename, string content)
    {
        var parsers = _parsers.Where(_ => _.CanParse(type, filename, content)).ToArray();
        ThrowIfMultipleParsersForConfigurationFile(filename, parsers);
        ThrowIfMissingParserForConfigurationFile(filename, parsers);

        var parser = parsers.Single();
        return parser.Parse(type, filename, content);
    }

    static void ThrowIfMultipleParsersForConfigurationFile(string filename, ICanParseConfigurationFile[] parsers)
    {
        if (parsers.Length > 1)
        {
            throw new MultipleParsersForConfigurationFile(filename);
        }
    }

    static void ThrowIfMissingParserForConfigurationFile(string filename, ICanParseConfigurationFile[] parsers)
    {
        if (parsers.Length == 0)
        {
            throw new MissingParserForConfigurationFile(filename);
        }
    }
}
