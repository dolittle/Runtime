// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines a system that can provide instances of <see cref="ILogMessageWriterCreator"/>.
    /// </summary>
    public interface ICanProvideLogMessageWriterCreators
    {
        /// <summary>
        /// Provides creators that creates instances of <see cref="ILogMessageWriter"/>.
        /// </summary>
        /// <returns>A set of <see cref="ILogMessageWriterCreator"/>.</returns>
        IEnumerable<ILogMessageWriterCreator> Provide();
    }
}
