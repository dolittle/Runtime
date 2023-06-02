// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Dolittle.Runtime.MongoDB;

public static class MongoClientSettingsExtensions
{
    static readonly HashSet<string> _commandsToFilter = new(new[] { "ping", "isMaster", "buildInfo", "saslStart", "saslContinue" },
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes <see cref="DiagnosticsActivityEventSubscriber"/>, which creates traces based on MongoDB events, and adds it as subscriber to <see cref="ClusterBuilder"/>.
    /// </summary>
    /// <param name="builder">The MongoDB ClusterBuilder.</param>
    public static void AddTelemetry(this ClusterBuilder builder)
    {
        builder.Subscribe(
            new DiagnosticsActivityEventSubscriber(
                new InstrumentationOptions
                {
                    ShouldStartActivity = commandStartedEvent => !_commandsToFilter.Contains(
                        commandStartedEvent.CommandName)
                }));
    }
}
