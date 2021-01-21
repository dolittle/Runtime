// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Security;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given
{
    public static class execution_contexts
    {
        public static ExecutionContext create() =>
            new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Versioning.Version.NotSet,
                "",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture);

        public static Execution.Contracts.ExecutionContext create_protobuf() => create().ToProtobuf();
    }
}
