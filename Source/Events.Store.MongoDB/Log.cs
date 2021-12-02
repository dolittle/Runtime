// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    static partial class Log
    {
        [LoggerMessage(0, LogLevel.Trace, "Creating indexes for the {Collection} collection in MongoDB Event Store")]
        internal static partial void CreatingIndexesFor(ILogger logger, string collection);
    }
}
