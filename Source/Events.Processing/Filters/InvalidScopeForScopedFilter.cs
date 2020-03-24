// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when the scope value of a scoped filter is invalid.
    /// </summary>
    public class InvalidScopeForScopedFilter : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScopeForScopedFilter"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        public InvalidScopeForScopedFilter(ScopeId scope)
            : base($"Scope for scoped filter cannot be '{scope}")
        {
        }
    }
}