// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Security;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class execution_contexts
    {
        public static ExecutionContext create_store() => create().ToStoreRepresentation();

        public static Execution.ExecutionContext create() =>
            new Execution.ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Versioning.Version.NotSet,
                "",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture);
    }
}