// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, string, Exception> _creatingIndexesFor = LoggerMessage
            .Define<string>(
                LogLevel.Trace,
                new EventId(1224270053, nameof(CreatingIndexesFor)),
                "Creating indexes for the {CollectionName} collection in MongoDB Event Store");

        internal static void CreatingIndexesFor(this ILogger logger, string collectionName)
            => _creatingIndexesFor(logger, collectionName, null);
    }
}