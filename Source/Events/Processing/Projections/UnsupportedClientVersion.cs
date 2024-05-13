// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections;

public class UnsupportedClientVersion: Exception
{
    public UnsupportedClientVersion(string message): base(message) {}
}
