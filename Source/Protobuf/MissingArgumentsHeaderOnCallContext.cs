// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Protobuf;

/// <summary>
/// Exception that gets thrown when a <see cref="ServerCallContext"/> is missing a required arguments message in the header.
/// </summary>
public class MissingArgumentsHeaderOnCallContext : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingArgumentsHeaderOnCallContext"/> class.
    /// </summary>
    /// <param name="callContext">The <see cref="ServerCallContext"/> where header is missing.</param>
    public MissingArgumentsHeaderOnCallContext(ServerCallContext callContext)
        : base($"Missing arguments header on call of server method '{callContext.Method}'")
    {
    }
}