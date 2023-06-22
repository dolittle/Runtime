// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

public record InitiateDisconnect
{
    public required TimeSpan? GracePeriod { get; init; }
}
