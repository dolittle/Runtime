// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Defines the signature of a reverse call context received event.
    /// </summary>
    public delegate void ReverseCallContextReceived(ReverseCallArgumentsContext context);
}
