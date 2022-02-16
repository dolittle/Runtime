// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion;


namespace Dolittle.Runtime.Security;

/// <summary>
/// Represents an implementation of <see cref="ISecurityManager"/>.
/// </summary>
[Singleton]
public class SecurityManager : ISecurityManager
{
    readonly IEnumerable<ISecurityDescriptor> _securityDescriptors;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityManager"/> class.
    /// </summary>
    /// <param name="securityDescriptors"><see cref="IEnumerable{ISecurityDescriptor}">Instances of security descriptors</see>.</param>
    public SecurityManager(IEnumerable<ISecurityDescriptor> securityDescriptors)
    {
        _securityDescriptors = securityDescriptors;
    }

    /// <inheritdoc/>
    public AuthorizationResult Authorize<T>(object target)
        where T : ISecurityAction
    {
        var result = new AuthorizationResult();
        if (!_securityDescriptors.Any())
        {
            return result;
        }

        var applicableSecurityDescriptors = _securityDescriptors.Where(sd => sd.CanAuthorize<T>(target));

        if (!applicableSecurityDescriptors.Any())
        {
            return result;
        }

        foreach (var securityDescriptor in applicableSecurityDescriptors)
            result.ProcessAuthorizeDescriptorResult(securityDescriptor.Authorize(target));

        return result;
    }
}
