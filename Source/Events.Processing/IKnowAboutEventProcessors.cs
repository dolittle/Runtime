// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that knows about <see cref="IEventProcessor">event processors</see>.
    /// </summary>
    public interface IKnowAboutEventProcessors : IEnumerable<IEventProcessor>
    {
    }
}
