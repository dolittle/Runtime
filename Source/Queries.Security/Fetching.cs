/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Security;
using doLittle.ReadModels;

namespace doLittle.Queries.Security
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
