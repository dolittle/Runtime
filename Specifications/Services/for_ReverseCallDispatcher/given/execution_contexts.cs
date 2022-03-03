// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using ExecutionContracts = Dolittle.Execution.Contracts;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;

public static class execution_contexts
{
    public static ExecutionContext create() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Version.NotSet,
            "",
            Guid.NewGuid(),
            Claims.Empty,
            CultureInfo.InvariantCulture);

    public static ExecutionContracts.ExecutionContext create_protobuf() => create().ToProtobuf();
}