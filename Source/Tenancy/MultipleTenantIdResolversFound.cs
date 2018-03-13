/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// The exception that is thrown if there are multiple implementations of <see cref="ICanResolveTenantId"/>
    /// </summary>
    public class MultipleTenantIdResolversFound : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultipleTenantIdResolversFound"/>
        /// </summary>
        public MultipleTenantIdResolversFound() : base("There can only be one implementation of ICanResolveTenantId - Highlander principle applied; there can be only one") { }
    }
}
