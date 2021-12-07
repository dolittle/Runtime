// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Security;

/// <summary>
/// Defines a builder for building a <see cref="ISecurityDescriptor"/>.
/// </summary>
public class SecurityDescriptorBuilder : ISecurityDescriptorBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityDescriptorBuilder"/> class.
    /// </summary>
    /// <param name="descriptor">The <see cref="ISecurityDescriptor"/> we are building.</param>
    public SecurityDescriptorBuilder(ISecurityDescriptor descriptor)
    {
        Descriptor = descriptor;
    }

    /// <inheritdoc/>
    public ISecurityDescriptor Descriptor { get; }
}