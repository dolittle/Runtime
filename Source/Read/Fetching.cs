/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Security;

namespace doLittle.Read
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for fetching <see cref="IReadModel">read models</see> 
    /// </summary>
    public class Fetching : SecurityAction
    {
#pragma warning disable 1591 // Xml Comments
        public override string ActionType
        {
            get { return "Fetching"; }
        }
#pragma warning restore 1591 // Xml Comments
    }
}
