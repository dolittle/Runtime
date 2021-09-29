// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a resilient <see cref="IStreamProcessorStateRepository" />.
    /// </summary>
    public interface IResilientStreamProcessorStateRepository : IStreamProcessorStateRepository
    {
    }
}
