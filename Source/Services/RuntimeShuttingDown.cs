// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

public class RuntimeShuttingDown : Exception
{
    public RuntimeShuttingDown() : base("Operation is unavailable: runtime is shutting down")
    {
    }

    public RuntimeShuttingDown(string message) : base(message)
    {
    }
}
