// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="ScopeId" /> of an scoped event handler is invalid.
    /// </summary>
    public class InvalidScopeForScopedEventHandler : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScopeForScopedEventHandler"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        public InvalidScopeForScopedEventHandler(ScopeId scope)
            : base($"Scope for scoped event handler cannot be '{scope}")
        {
        }
    }
}