// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ReadModels;
using Dolittle.Security;

namespace Dolittle.Queries.Security
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchingSecurityManager"/>.
    /// </summary>
    public class FetchingSecurityManager : IFetchingSecurityManager
    {
        readonly ISecurityManager _securityManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchingSecurityManager"/> class.
        /// </summary>
        /// <param name="securityManager"><see cref="ISecurityManager"/> for forwarding requests related to security to.</param>
        public FetchingSecurityManager(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        /// <inheritdoc/>
        public AuthorizationResult Authorize<T>(IReadModelOf<T> readModelOf)
            where T : IReadModel
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
