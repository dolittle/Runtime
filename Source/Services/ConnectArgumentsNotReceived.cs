// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Exception that gets thrown when connect arguments was not received as the first message from a reverse call client.
/// </summary>
public class ConnectArgumentsNotReceived : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConnectArgumentsNotReceived" /> class.
    /// </summary>
    public ConnectArgumentsNotReceived()
        : base($"Reverse call connect arguments not received")
    {
    }
}