// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Dolittle.Runtime.CLI.Runtime.EventHandlers;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.EventHandlers
{
    /// <summary>
    /// An implementation of <see cref="IValueParser"/> that parses instances of <see cref="MicroserviceAddress"/>.
    /// </summary>
    public class EventHandlerIdOrAliasParser : IValueParser
    {
        /// <inheritdoc />
        public Type TargetType => typeof(EventHandlerIdOrAlias);

        /// <inheritdoc />
        public object Parse(string argName, string value, CultureInfo culture)
        {
            var scope = ScopeId.Default;
            var segments = value.Split(":");
            ThrowIfInvalidFormat(value, segments);
            if (segments.Length > 1)
            {
                scope = Guid.Parse(segments[1]);
            }

            return Guid.TryParse(segments[0], out var eventHandlerId)
                ? new EventHandlerIdOrAlias(new EventHandlerId(scope, eventHandlerId))
                : new EventHandlerIdOrAlias(segments[0], scope);
        }

        static void ThrowIfInvalidFormat(string value, string[] segments)
        {
            switch (segments.Length)
            {
                case > 2 or 0:
                    throw new InvalidEventHandlerIdOrAlias(value);
                case > 1 when !Guid.TryParse(segments[1], out _):
                    throw new InvalidEventHandlerIdOrAlias(value);
            }
        }
    }
}
