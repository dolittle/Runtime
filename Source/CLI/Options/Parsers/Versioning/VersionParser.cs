// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace CLI.Options.Parsers.Versioning
{
    public class VersionParser : IValueParser
    {
        readonly IVersionConverter _converter;

        public VersionParser(IVersionConverter converter)
        {
            _converter = converter;
        }

        public Type TargetType => typeof(Dolittle.Runtime.Versioning.Version);

        public object Parse(string argName, string value, CultureInfo culture)
            => _converter.FromString(value);
    }
}