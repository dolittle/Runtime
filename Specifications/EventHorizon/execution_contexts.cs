// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Security;
using ExecutionContracts = Dolittle.Execution.Contracts;

namespace Dolittle.Runtime.EventHorizon
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

        public static ExecutionContext create_with_claims(Claims claims) =>
            new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Versioning.Version.NotSet,
                "",
                Guid.NewGuid(),
                claims,
                CultureInfo.InvariantCulture);

        public static ExecutionContracts.ExecutionContext create_protobuf() => create().ToProtobuf();
    }
}
