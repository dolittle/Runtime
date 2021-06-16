// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that knows about <see cref="Client">clients</see>.
    /// </summary>
    public interface IKnowAboutClients
    {
        /// <summary>
        /// Gets the <see cref="IEnumerable{T}">collection</see> of <see cref="Client"/>.
        /// </summary>
        IEnumerable<Client> Clients { get; }
    }
}