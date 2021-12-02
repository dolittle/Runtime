// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Exception that gets thrown when ping timed out.
/// </summary>
public class PingTimedOut : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PingTimedOut"/> class.
    /// </summary>
    public PingTimedOut()
        : base("Ping timed out")
    {
    }
}