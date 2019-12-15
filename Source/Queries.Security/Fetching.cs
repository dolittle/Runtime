// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ReadModels;
using Dolittle.Security;

namespace Dolittle.Queries.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for fetching <see cref="IReadModel">read models</see>.
    /// </summary>
    public class Fetching : SecurityAction
    {
        /// <inheritdoc/>
        public override string ActionType
        {
            get { return "Fetching"; }
        }
    }
}
