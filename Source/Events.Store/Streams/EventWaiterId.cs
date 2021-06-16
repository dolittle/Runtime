// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents the unique identifier for an <see cref="EventWaiter" />.
    /// </summary>
    public record EventWaiterId(ScopeId Scope, StreamId Stream);
}
