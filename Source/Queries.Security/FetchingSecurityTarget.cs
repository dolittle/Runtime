// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;

namespace Dolittle.Queries.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityTarget">security target</see> for <see cref="Fetching"/>.
    /// </summary>
    public class FetchingSecurityTarget : SecurityTarget
    {
        const string Fetching = "Fetching";

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchingSecurityTarget"/> class.
        /// </summary>
        public FetchingSecurityTarget()
            : base(Fetching)
        {
        }
    }
}
