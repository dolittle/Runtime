/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Concepts;

namespace doLittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an identifier for <see cref="ITenant"/>
    /// </summary>
    public class TenantId : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="TenantId"/>
        /// </summary>
        /// <param name="tenantId"></param>
        public static implicit operator TenantId(string tenantId)
        {
            return new TenantId { Value = tenantId };
        }
    }
}
