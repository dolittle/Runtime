/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Security;
using Dolittle.ReadModels;

namespace Dolittle.Queries.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for fetching <see cref="IReadModel">read models</see> 
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
