// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Dolittle.Runtime.Microservices;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.Microservices
{
    /// <summary>
    /// An implementation of <see cref="IValueParser"/> that parses instances of <see cref="MicroserviceAddress"/>.
    /// </summary>
    public class MicroserviceAddressParser : IValueParser
    {
        static readonly Regex _hostPortExpression = new(@"^(?<host>(?>[^:]*)|(?>\[[^\]]*\])|(?>(?>[a-fA-F0-9]{0,4}:){7}(?>[a-fA-F0-9]{0,4})))(?<port>:\d*)?$", RegexOptions.Compiled | RegexOptions.Singleline);
        
        /// <inheritdoc />
        public Type TargetType => typeof(MicroserviceAddress);

        /// <inheritdoc />
        public object Parse(string argName, string value, CultureInfo culture)
        {
            var match = _hostPortExpression.Match(value);
            if (!match.Success)
            {
                throw new InvalidMicroserviceAddress(value);
            }

            var hostGroup = match.Groups["host"];
            var portGroup = match.Groups["port"];

            var host = hostGroup.Value;
            var port = portGroup.Success ? int.Parse(portGroup.Value.TrimStart(':'), culture) : 0;

            return new MicroserviceAddress(host, port);
        }
    }
}
