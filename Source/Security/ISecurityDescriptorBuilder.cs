// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Defines the builder for building a <see cref="ISecurityDescriptor"/>.
    /// </summary>
    public interface ISecurityDescriptorBuilder
    {
        /// <summary>
        /// Gets the <see cref="ISecurityDescriptor"/> that is used by the builder.
        /// </summary>
        ISecurityDescriptor Descriptor { get; }
    }
}
