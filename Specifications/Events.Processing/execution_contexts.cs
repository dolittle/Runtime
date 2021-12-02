// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Security;

namespace Dolittle.Runtime.Events.Processing;

public static class execution_contexts
{
    public static ExecutionContext create() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Versioning.Version.NotSet,
            "",
            Guid.NewGuid(),
            Claims.Empty,
            CultureInfo.InvariantCulture);
}