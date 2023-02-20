// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.Runtime.Execution;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Events.Store;

public static class execution_contexts
{
    public static ExecutionContext create() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Version.NotSet,
            "",
            Guid.NewGuid(),
            ActivitySpanId.CreateRandom(),
            Claims.Empty,
            CultureInfo.InvariantCulture);
}