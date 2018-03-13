/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Security;
using Dolittle.ReadModels;

namespace Dolittle.Queries.Security
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchingSecurityManager"/>
    /// </summary>
    public class FetchingSecurityManager : IFetchingSecurityManager
    {
        readonly ISecurityManager _securityManager;

        /// <summary>
        /// Initializes a new instance of <see cref="FetchingSecurityManager"/>
        /// </summary>
        /// <param name="securityManager"><see cref="ISecurityManager"/> for forwarding requests related to security to</param>
        public FetchingSecurityManager(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        /// <inheritdoc/>
        public AuthorizationResult Authorize<T>(IReadModelOf<T> readModelOf) where T : IReadModel
        {
            return _securityManager.Authorize<Fetching>(readModelOf);
        }

        /// <inheritdoc/>
        public AuthorizationResult Authorize(IQuery query)
        {
            return _securityManager.Authorize<Fetching>(query);
        }
    }
}
