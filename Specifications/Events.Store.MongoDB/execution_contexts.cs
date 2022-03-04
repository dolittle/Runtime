// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Execution;
using ExecutionContext = Dolittle.Runtime.Events.Store.MongoDB.Events.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public static class execution_contexts
{
    public static ExecutionContext create_store() => create().ToStoreRepresentation();

    public static Execution.ExecutionContext create() =>
        new(
            Guid.Parse("1cec3f3c-1752-4210-bcd0-5add96e7172d"),
            Guid.Parse("86e7a8fc-b0e9-4709-bf59-10b33005497e"),
            new Version(873112588, 241520971, 367002811, 1885758720, "something very random"),
            "somethign also very random",
            Guid.Parse("2fd440e7-84c6-4f88-a5f3-f8b5a038464f"),
            new Claims(new[] { new Execution.Claim("some very random name", "some very random value", "some very random value type") }),
            CultureInfo.InvariantCulture);
}