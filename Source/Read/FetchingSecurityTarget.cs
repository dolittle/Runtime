/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Security;

namespace doLittle.Read
{
    /// <summary>
    /// Represents a <see cref="ISecurityTarget">security target</see> for <see cref="Fetching"/>
    /// </summary>
    public class FetchingSecurityTarget : SecurityTarget
    {
        const string Fetching = "Fetching";

        /// <summary>
        /// Initializes an instance of <see cref="FetchingSecurityTarget"/>
        /// </summary>
        public FetchingSecurityTarget() : base(Fetching) { }
    }
}
