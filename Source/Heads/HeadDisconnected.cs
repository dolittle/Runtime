// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents the delegate for when a <see cref="Head"/> gets disconnected.
    /// </summary>
    /// <param name="head"><see cref="Head"/> that gets disconnected.</param>
    public delegate void HeadDisconnected(Head head);
}